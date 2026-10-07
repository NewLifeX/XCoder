using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using NewLife.Collections;

namespace ALib
{
    /// <summary>环形队列接口</summary>
    public interface ICircularQueue<T>
    {
        /// <summary>进队</summary>
        /// <param name="item"></param>
        /// <param name="expan">是否扩容</param>
        /// <returns></returns>
        public bool Enqueue(T item, bool expan = false);

        /// <summary>出队</summary>
        /// <returns></returns>
        public T Dequeue();

        /// <summary>设置容量</summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public bool SetCapacity(int size);

        /// <summary>丢弃，单向的。</summary>
        /// <remarks>环形队列的特殊性，seek出来的位置会被head写入。必须不能倒退</remarks>
        /// <param name="capacity">负值清空所有</param>
        public void Seek(int capacity);

        /// <summary>序列化为数组，队列内容不丢失</summary>
        /// <returns></returns>
        public T[] ToArray();

        /// <summary>进队多个</summary>
        /// <param name="item"></param>
        /// <param name="expan">是否扩容</param>
        /// <returns></returns>
        public bool WriteArray(T[] item, bool expan = false);

        /// <summary>出队多个</summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public T[] ReadArray(int size = -1);
    }

    /// <summary>环形队列，不带锁</summary>
    /// <remarks>写、读 不冲突。多读/多写 冲突。</remarks>
    /// <typeparam name="T"></typeparam>
    public class CircularQueue<T> : ICircularQueue<T>
    {
        /// <summary>内部缓冲</summary>
        protected T[] _queue;
        /// <summary>头，写入下标</summary>
        protected volatile int _head;
        /// <summary>尾，读取下标</summary>
        protected volatile int _tail;

        // <summary>不使用 count 变量维护内部数量，避免读写操作count变量造成竞争</summary>
        // protected int _count;

        /// <summary>是否为基元类型，决定是否可用 Buffer.BlockCopy</summary>
        protected static readonly bool _isPrimitive = typeof(T).IsPrimitive;

        /// <summary>每个元素的字节大小，用于 Buffer.BlockCopy</summary>
        protected static readonly int _sizeOfT = _isPrimitive ? System.Runtime.InteropServices.Marshal.SizeOf<T>() : -1;

        /// <summary>复制数组，根据类型选择最佳方式</summary>
        private void CopyArray(T[] src, int srcIdx, T[] dst, int dstIdx, int len)
        {
            if (_isPrimitive)
            {
                Buffer.BlockCopy(src, srcIdx * _sizeOfT, dst, dstIdx * _sizeOfT, len * _sizeOfT);
            }
            else
            {
                Array.Copy(src, srcIdx, dst, dstIdx, len);
            }
        }

        /// <summary>构造</summary>
        /// <param name="capacity">最少32个吧</param>
        public CircularQueue(int capacity = -1)
        {
            if (capacity < 8) capacity = 8;

            // 必须留一个空位置，否则无法使用 _head  _tial 区分空和满。
            _queue = new T[capacity + 1];
            _head = 0;
            _tail = 0;
        }

        /// <summary>清空内容</summary>
        public void Clear()
        {
            lock (this)
            {
                Array.Clear(_queue, 0, _queue.Length);

                _head = 0;
                _tail = 0;
            }
        }

        /// <summary>队列内容量</summary>
        /// <returns></returns>
        public int Count()
        {
            // 变量copy处理，避免计算过程中其他线程修改。
            var head = _head;
            var tail = _tail;

            if (head >= tail) return head - tail;

            return head + _queue.Length - tail;
        }

        /// <summary>剩余空间</summary>
        /// <returns></returns>
        public int Remain()
        {
            // 必须留一个空位置，否则无法使用 _head  _tial 区分空和满。
            return _queue.Length - Count() - 1;
        }

        /// <summary>空</summary>
        public bool IsEmpty()
        {
            return _tail == _head;
        }

        /// <summary>满的</summary>
        public bool IsFull()
        {
            return Remain() == 0;
        }

        /// <summary>进队</summary>
        /// <param name="item"></param>
        /// <param name="expan">是否扩容</param>
        /// <returns></returns>
        public virtual bool Enqueue(T item, bool expan = false)
        {
            if (IsFull())
            {
                if (!expan) return false;
                // 扩容一倍
                SetCapacity((_queue.Length - 1) * 2);
            }

            _queue[_head] = item;

            var head = _head + 1;
            // _head = head % _queue.Length;
            // 语法上看，取余是最好的写法。但是执行效率不一定合适。
            // 如果是ARM处理器，取余/除法 执行效率都不高。
            // 还是加减法吧。
            if (head < _queue.Length)
                _head = head;
            else
                _head = head - _queue.Length;

            return true;
        }

        /// <summary>出队</summary>
        /// <returns></returns>
        public virtual T Dequeue()
        {
            if (IsEmpty()) return default(T);

            var temp = _queue[_tail];
            _queue[_tail] = default(T);

            var tail = _tail + 1;
            // _tail = tail % _queue.Length;
            // 语法上看，取余是最好的写法。但是执行效率不一定合适。
            // 如果是ARM处理器，取余/除法 执行效率都不高。
            // 还是加减法吧。
            if (tail < _queue.Length)
                _tail = tail;
            else
                _tail = tail - _queue.Length;

            return temp;
        }

        /// <summary>读一个，但不出队</summary>
        /// <returns></returns>
        public T Peek()
        {
            if (IsEmpty()) return default(T);

            return _queue[_tail];
        }

        /// <summary>读多个，但不出队列</summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public T[] PeekArray(int size = -1)
        {
            var cnt = Count();
            var len = (size == -1) ? cnt : size;
            if (len > cnt) len = cnt;

            if (len == 0) return new T[0];

            // 变量copy处理，避免计算过程中其他线程修改。
            var tail = _tail;
            var result = new T[len];

            if (tail + len <= _queue.Length)
            {
                CopyArray(_queue, tail, result, 0, len);
            }
            else
            {
                // _queue 后半截
                var cplen = _queue.Length - tail;
                CopyArray(_queue, tail, result, 0, cplen);

                // _queue 前半截
                var cplen2 = len - cplen;
                CopyArray(_queue, 0, result, cplen, cplen2);
            }

            return result;
        }


        /// <summary>读最后写入的一个，不出队（不允许从_head出队）</summary>
        /// <returns></returns>
        public T PeekHead()
        {
            if (IsEmpty()) return default(T);

            var n = _queue.Length;
            var idx = _head - 1;
            if (idx < 0) idx += n;

            return _queue[idx];
        }

        /// <summary>读最后最后写入的多个，不出队（不允许从_head出队）</summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public T[] PeekHeadArray(int size = -1)
        {
            var cnt = Count();
            var len = (size == -1) ? cnt : size;
            if (len > cnt) len = cnt;
            if (len == 0) return new T[0];

            var n = _queue.Length;
            // 最近写入的元素在 (_head - 1)，窗口起点（最旧的一个）为 (_head - len)
            var start = _head - len;
            if (start < 0) start += n;

            var result = new T[len];

            if (start + len <= n)
            {
                CopyArray(_queue, start, result, 0, len);
            }
            else
            {
                var cplen = n - start;
                CopyArray(_queue, start, result, 0, cplen);

                var cplen2 = len - cplen;
                CopyArray(_queue, 0, result, cplen, cplen2);
            }

            return result;
        }

        /// <summary>进队多个</summary>
        /// <param name="item"></param>
        /// <param name="expan">是否扩容</param>
        /// <returns></returns>
        public virtual bool WriteArray(T[] item, bool expan = false)
        {
            var remian = Remain();
            var wrlen = item.Length;
            if (remian < wrlen)
            {
                if (!expan) return false;

                // 计算扩容到多少才够
                var needlen = Count() + wrlen;
                var len = _queue.Length - 1;
                while (true)
                {
                    len *= 2;
                    if (needlen < len) break;
                }

                SetCapacity(len);
            }

            var head = _head;
            if (head + wrlen <= _queue.Length)
            {
                CopyArray(item, 0, _queue, head, wrlen);
            }
            else
            {
                // 后半部
                var cplen = _queue.Length - head;
                CopyArray(item, 0, _queue, head, cplen);
                // 前半部
                var cplen2 = wrlen - cplen;
                CopyArray(item, cplen, _queue, 0, cplen2);
            }

            head += wrlen;
            // _head = head % _queue.Length;
            // 语法上看，取余是最好的写法。但是执行效率不一定合适。
            // 如果是ARM处理器，取余/除法 执行效率都不高。
            // 还是加减法吧。
            if (head < _queue.Length)
                _head = head;
            else
                _head = head - _queue.Length;

            return true;
        }

        /// <summary>出队多个</summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public virtual T[] ReadArray(int size = -1)
        {
            var result = PeekArray(size);

            var tail = _tail;
            var len = result.Length;

            tail += len;
            // _tail = tail % _queue.Length;
            // 语法上看，取余是最好的写法。但是执行效率不一定合适。
            // 如果是ARM处理器，取余/除法 执行效率都不高。
            // 还是加减法吧。
            if (tail < _queue.Length)
                _tail = tail;
            else
                _tail = tail - _queue.Length;

            return result;
        }

        /// <summary>设置容量</summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public bool SetCapacity(int size)
        {
            lock (this)
            {
                var cnt = Count();
                // 设置容量不能小于当前队列内容大小。
                if (size < cnt) return false;

                // 读出内容
                var all = ToArray();

                // 新建缓冲
                // 必须+1留一个空位置，否则无法使用 _head  _tial 区分空和满。
                var queue = new T[size + 1];
                // 放入新缓冲
                CopyArray(all, 0, queue, 0, all.Length);

                // 更新 queue tail head
                _queue = queue;
                _tail = 0;
                _head = all.Length;
            }

            return true;
        }

        /// <summary>丢弃，单向的。</summary>
        /// <remarks>环形队列的特殊性，seek出来的位置会被head写入。必须不能倒退</remarks>
        /// <param name="capacity">负值清空所有</param>
        public virtual void Seek(int capacity)
        {
            if (capacity < 0)
            {
                Clear();
                return;
            }

            var cnt = Count();
            var len = cnt > capacity ? capacity : cnt;
            if (len == 0) return;

            // 变量copy处理，避免计算过程中其他线程修改。
            var tail = _tail;
            if (tail + len <= _queue.Length)
            {
                Array.Clear(_queue, tail, len);
            }
            else
            {
                // _queue 后半截
                var cplen = _queue.Length - tail;
                Array.Clear(_queue, tail, cplen);

                // _queue 前半截
                var cplen2 = len - cplen;
                Array.Clear(_queue, 0, cplen2);
            }

            tail += len;
            // _tail = tail % _queue.Length;
            // 语法上看，取余是最好的写法。但是执行效率不一定合适。
            // 如果是ARM处理器，取余/除法 执行效率都不高。
            // 还是加减法吧。
            if (tail < _queue.Length)
                _tail = tail;
            else
                _tail = tail - _queue.Length;
        }

        /// <summary>序列化为数组，队列内容不丢失</summary>
        /// <returns></returns>
        public virtual T[] ToArray()
        {
            var len = Count();
            if (len == 0) return new T[0];

            // 变量copy处理，避免计算过程中其他线程修改。
            var tail = _tail;
            var result = new T[len];

            if (tail + len <= _queue.Length)
            {
                CopyArray(_queue, tail, result, 0, len);
            }
            else
            {
                // _queue 后半截
                var cplen = _queue.Length - tail;
                CopyArray(_queue, tail, result, 0, cplen);

                // _queue 前半截
                var cplen2 = len - cplen;
                CopyArray(_queue, 0, result, cplen, cplen2);
            }

            return result;
        }
    }
}
