using System.Text;
using XCode.DataAccessLayer;

namespace XCoderMac.Services;

/// <summary>
/// Windows 数据工具核心操作的 Mac 适配层。
/// 全部调用 XCode 自带的 DAL.Import / Export / Metadata，且不会悄悄修改数据库。
/// </summary>
public static class MacDataToolsService
{
    public static List<IDataTable> ImportXml(string xml)
    {
        if (string.IsNullOrWhiteSpace(xml)) throw new InvalidDataException("XML 文件为空");
        var result = DAL.Import(xml) ?? throw new InvalidDataException("XCode 未返回任何模型");
        return result.ToList();
    }

    public static List<IDataTable> ImportFile(string path) =>
        ImportXml(File.ReadAllText(path));

    public static string ExportXml(IEnumerable<IDataTable> tables)
    {
        var list = tables.ToList();
        if (list.Count == 0) throw new InvalidOperationException("当前没有可导出的数据表");
        return DAL.Export(list);
    }

    // 编辑副本：取消编辑时不会污染主窗体原始数据库表对象。
    public static List<IDataTable> CloneModel(IEnumerable<IDataTable> tables) =>
        ImportXml(ExportXml(tables));

    public static string CreateTableSql(IMetaData metadata, IDataTable table)
    {
        var builder = new StringBuilder();
        void Add(DDLSchema action, object obj)
        {
            var sql = metadata.GetSchemaSQL(action, obj);
            if (string.IsNullOrWhiteSpace(sql)) return;
            builder.Append(sql.Trim().TrimEnd(';')).AppendLine(";").AppendLine();
        }
        Add(DDLSchema.CreateTable, table);
        if (!string.IsNullOrWhiteSpace(table.Description))
            Add(DDLSchema.AddTableDescription, table);
        foreach (var column in table.Columns)
            if (!string.IsNullOrWhiteSpace(column.Description))
                Add(DDLSchema.AddColumnDescription, column);
        foreach (var index in table.Indexes)
            if (!index.PrimaryKey)
                Add(DDLSchema.CreateIndex, index);
        return builder.ToString();
    }

    public static string CreateAllSql(IMetaData metadata, IEnumerable<IDataTable> tables) =>
        string.Join(Environment.NewLine, tables.Select(t => CreateTableSql(metadata, t)));

    public static void ValidateModel(IEnumerable<IDataTable> tables)
    {
        var all = tables.ToList();
        if (all.Count == 0) throw new InvalidOperationException("模型中至少需要一张表");
        if (all.Any(t => string.IsNullOrWhiteSpace(t.Name) ||
                         string.IsNullOrWhiteSpace(t.TableName)))
            throw new InvalidOperationException("表名和实体名称不能为空");
        if (all.GroupBy(t => t.TableName, StringComparer.OrdinalIgnoreCase).Any(g => g.Count() > 1))
            throw new InvalidOperationException("模型含有重复的数据库表名");
        foreach (var table in all)
        {
            if (table.Columns.GroupBy(c => c.ColumnName, StringComparer.OrdinalIgnoreCase)
                .Any(g => g.Count() > 1))
                throw new InvalidOperationException($"数据表 {table.Name} 中存在重复字段名");
            if (table.Columns.Any(c => string.IsNullOrWhiteSpace(c.Name) ||
                                       string.IsNullOrWhiteSpace(c.ColumnName)))
                throw new InvalidOperationException($"数据表 {table.Name} 存在空字段名");
            foreach (var index in table.Indexes)
            {
                if (index.Columns is not { Length: > 0 } ||
                    index.Columns.Any(name => table.Columns.All(c =>
                        !c.ColumnName.Equals(name, StringComparison.OrdinalIgnoreCase))))
                    throw new InvalidOperationException($"表 {table.Name} 中索引 {index.Name} 引用了不存在的字段");
            }
        }
    }
}
