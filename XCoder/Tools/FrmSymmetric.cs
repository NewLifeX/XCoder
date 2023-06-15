using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;
using NewLife;
using XCoder.Common;

namespace XCoder.Tools;

[Category("安全工具")]
[DisplayName("对称加密")]
public partial class FrmSymmetric : Form, IXForm
{
    private ControlConfig _config;

    #region 窗体初始化
    public FrmSymmetric()
    {
        InitializeComponent();

        // 动态调节宽度高度，兼容高DPI
        this.FixDpi();
    }

    private void FrmSecurity_Load(Object sender, EventArgs e)
    {
        cmbCipher.DataSource = Enum.GetValues(typeof(CipherMode));
        cmbPadding.DataSource = Enum.GetValues(typeof(PaddingMode));
        cmbCipher.SelectedItem = CipherMode.CBC;
        cmbPadding.SelectedItem = PaddingMode.PKCS7;

        _config = new ControlConfig { Control = this, FileName = "Symmetric.json" };
        _config.Load();
    }
    #endregion

    #region 辅助
    /// <summary>从字符串中获取字节数组</summary>
    /// <param name="str"></param>
    /// <returns></returns>
    private Byte[] GetBytes(String str)
    {
        if (str.IsNullOrEmpty()) return new Byte[0];

        try
        {
            if (str.Contains("-")) return str.ToHex();
        }
        catch { }

        try
        {
            return str.ToBase64();
        }
        catch { }

        return str.GetBytes();
    }

    /// <summary>从原文中获取字节数组</summary>
    /// <returns></returns>
    private Byte[] GetSource()
    {
        var v = rtSource.Text;

        if (rbString.Checked) return v.GetBytes();
        if (rbHex.Checked) return v.ToHex();
        if (rbBase64.Checked) return v.ToBase64();

        return null;
    }

    private void rtSource_TextChanged(Object sender, EventArgs e)
    {
        var v = rtSource.Text;
        if (v.IsNullOrEmpty()) return;

        // 单字节
        var enc = Encoding.UTF8;
        if (enc.GetByteCount(v) != v.Length)
        {
            rbHex.Enabled = false;
            rbBase64.Enabled = false;
            return;
        }

        try
        {
            rbHex.Enabled = v.ToHex().Length > 0;
        }
        catch
        {
            rbHex.Enabled = false;
        }

        try
        {
            rbBase64.Enabled = v.ToBase64().Length > 0;
        }
        catch
        {
            rbBase64.Enabled = false;
        }
    }

    private Byte[] GetPass()
    {
        var v = rtPass.Text;

        if (rbString2.Checked) return v.GetBytes();
        if (rbHex2.Checked) return v.ToHex();
        if (rbBase642.Checked) return v.ToBase64();

        return null;
    }

    private void SetResult(params String[] rs)
    {
        var sb = new StringBuilder();
        foreach (var item in rs)
        {
            if (sb.Length > 0) sb.AppendLine();
            sb.Append(item);
        }
        rtResult.Text = sb.ToString();

        _config.Save();
    }

    private void SetResult(Byte[] data)
    {
        //SetResult("/*HEX编码、Base64编码、Url改进Base64编码*/", data.ToHex("-"), data.ToBase64(), data.ToUrlBase64());

        var list = new List<String>();
        if (cbString.Checked) list.Add(data.ToStr());
        if (cbHex.Checked)
        {
            list.Add(data.ToHex().ToUpper());
            list.Add(data.ToHex().ToLower());
            list.Add(data.ToHex("-"));
            list.Add(data.ToHex(" "));
        }
        if (cbBase64.Checked)
        {
            list.Add(data.ToBase64());
            list.Add(data.ToUrlBase64());
        }

        SetResult(list.ToArray());
    }
    #endregion

    #region 功能
    private void btnExchange_Click(Object sender, EventArgs e)
    {
        var v = rtSource.Text;
        var v2 = rtResult.Text;
        // 结果区只要第一行
        if (!v2.IsNullOrEmpty())
        {
            var ss = v2.Split("\n");
            var n = 0;
            if (ss.Length > n + 1 && ss[n].StartsWith("/*") && ss[n].EndsWith("*/")) n++;
            v2 = ss[n];
        }
        rtSource.Text = v2;
        rtResult.Text = v;
    }

    private void btnDES_Click(Object sender, EventArgs e)
    {
        var buf = GetSource();
        var pass = GetPass();

        var mode = (CipherMode)Enum.Parse(typeof(CipherMode), cmbCipher.SelectedItem + "");
        var padding = (PaddingMode)Enum.Parse(typeof(PaddingMode), cmbPadding.SelectedItem + "");
        var des = new DESCryptoServiceProvider();
        buf = des.Encrypt(buf, pass, mode, padding);

        SetResult(buf);
    }

    private void btnDES2_Click(Object sender, EventArgs e)
    {
        var buf = GetSource();
        var pass = GetPass();

        var mode = (CipherMode)Enum.Parse(typeof(CipherMode), cmbCipher.SelectedItem + "");
        var padding = (PaddingMode)Enum.Parse(typeof(PaddingMode), cmbPadding.SelectedItem + "");
        var des = new DESCryptoServiceProvider();
        buf = des.Decrypt(buf, pass, mode, padding);

        SetResult(buf);
    }

    private void btnAES_Click(Object sender, EventArgs e)
    {
        var buf = GetSource();
        var pass = GetPass();

        var mode = (CipherMode)Enum.Parse(typeof(CipherMode), cmbCipher.SelectedItem + "");
        var padding = (PaddingMode)Enum.Parse(typeof(PaddingMode), cmbPadding.SelectedItem + "");
        var aes = Aes.Create();
        buf = aes.Encrypt(buf, pass, mode, padding);

        SetResult(buf);
    }

    private void btnAES2_Click(Object sender, EventArgs e)
    {
        var buf = GetSource();
        var pass = GetPass();

        var mode = (CipherMode)Enum.Parse(typeof(CipherMode), cmbCipher.SelectedItem + "");
        var padding = (PaddingMode)Enum.Parse(typeof(PaddingMode), cmbPadding.SelectedItem + "");
        var aes = Aes.Create();
        buf = aes.Decrypt(buf, pass, mode, padding);

        SetResult(buf);
    }

    private void btnRC4_Click(Object sender, EventArgs e)
    {
        var buf = GetSource();
        var pass = GetPass();
        buf = buf.RC4(pass);

        SetResult(buf);
    }

    private void btnRC42_Click(Object sender, EventArgs e)
    {
        var buf = GetSource();
        var pass = GetPass();
        buf = buf.RC4(pass);

        SetResult(buf);
    }

    private void btnRC2_Click(Object sender, EventArgs e)
    {
        var buf = GetSource();
        var pass = GetPass();

        var mode = (CipherMode)Enum.Parse(typeof(CipherMode), cmbCipher.SelectedItem + "");
        var padding = (PaddingMode)Enum.Parse(typeof(PaddingMode), cmbPadding.SelectedItem + "");
        buf = RC2.Create().Encrypt(buf, pass, mode, padding);

        SetResult(buf);
    }

    private void btnRC22_Click(Object sender, EventArgs e)
    {
        var buf = GetSource();
        var pass = GetPass();

        var mode = (CipherMode)Enum.Parse(typeof(CipherMode), cmbCipher.SelectedItem + "");
        var padding = (PaddingMode)Enum.Parse(typeof(PaddingMode), cmbPadding.SelectedItem + "");
        buf = RC2.Create().Decrypt(buf, pass, mode, padding);

        SetResult(buf);
    }

    private void btnRijndael_Click(Object sender, EventArgs e)
    {
        var buf = GetSource();
        var pass = GetPass();

        var mode = (CipherMode)Enum.Parse(typeof(CipherMode), cmbCipher.SelectedItem + "");
        var padding = (PaddingMode)Enum.Parse(typeof(PaddingMode), cmbPadding.SelectedItem + "");
        buf = Rijndael.Create().Encrypt(buf, pass, mode, padding);

        SetResult(buf);
    }

    private void btnRijndael2_Click(Object sender, EventArgs e)
    {
        var buf = GetSource();
        var pass = GetPass();

        var mode = (CipherMode)Enum.Parse(typeof(CipherMode), cmbCipher.SelectedItem + "");
        var padding = (PaddingMode)Enum.Parse(typeof(PaddingMode), cmbPadding.SelectedItem + "");
        buf = Rijndael.Create().Decrypt(buf, pass, mode, padding);

        SetResult(buf);
    }

    private void btn3DES_Click(Object sender, EventArgs e)
    {
        var buf = GetSource();
        var pass = GetPass();

        var mode = (CipherMode)Enum.Parse(typeof(CipherMode), cmbCipher.SelectedItem + "");
        var padding = (PaddingMode)Enum.Parse(typeof(PaddingMode), cmbPadding.SelectedItem + "");
        buf = TripleDES.Create().Encrypt(buf, pass, mode, padding);

        SetResult(buf);
    }

    private void btn3DES2_Click(Object sender, EventArgs e)
    {
        var buf = GetSource();
        var pass = GetPass();

        var mode = (CipherMode)Enum.Parse(typeof(CipherMode), cmbCipher.SelectedItem + "");
        var padding = (PaddingMode)Enum.Parse(typeof(PaddingMode), cmbPadding.SelectedItem + "");
        buf = TripleDES.Create().Decrypt(buf, pass, mode, padding);

        SetResult(buf);
    }

    private void btnSM4_Click(Object sender, EventArgs e)
    {
        var buf = GetSource();
        var pass = GetPass();

        var mode = (CipherMode)Enum.Parse(typeof(CipherMode), cmbCipher.SelectedItem + "");
        var padding = (PaddingMode)Enum.Parse(typeof(PaddingMode), cmbPadding.SelectedItem + "");
        buf = new SM4().Encrypt(buf, pass, mode, padding);

        SetResult(buf);
    }

    private void btnSM42_Click(Object sender, EventArgs e)
    {
        var buf = GetSource();
        var pass = GetPass();

        var mode = (CipherMode)Enum.Parse(typeof(CipherMode), cmbCipher.SelectedItem + "");
        var padding = (PaddingMode)Enum.Parse(typeof(PaddingMode), cmbPadding.SelectedItem + "");
        buf = new SM4().Decrypt(buf, pass, mode, padding);

        SetResult(buf);
    }
    #endregion
}