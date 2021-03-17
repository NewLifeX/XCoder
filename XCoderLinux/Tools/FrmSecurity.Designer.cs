using Gtk;

namespace XCoder.Tools
{
    partial class FrmSecurity
    {

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        [System.Obsolete]
        private void InitializeComponent()
        {
            this.gbFunc = new Frame();
            this.tableFunc = new Table(14, 2, true);
            this.btnTime = new Button();
            this.btnSHA512 = new Button();
            this.btnSHA384 = new Button();
            this.btnSHA256 = new Button();
            this.btnSHA1 = new Button();
            this.btnHtml2 = new Button();
            this.btnHtml = new Button();
            this.btnUrl2 = new Button();
            this.btnUrl = new Button();
            this.btnDSA2 = new Button();
            this.btnDSA = new Button();
            this.btnRSA2 = new Button();
            this.btnRSA = new Button();
            this.btnRC42 = new Button();
            this.btnRC4 = new Button();
            this.btnAES2 = new Button();
            this.btnAES = new Button();
            this.btnDES2 = new Button();
            this.btnDES = new Button();
            this.btnCRC2 = new Button();
            this.btnCRC = new Button();
            this.btnMD52 = new Button();
            this.btnMD5 = new Button();
            this.btnB642 = new Button();
            this.btnB64 = new Button();
            this.btnHex2 = new Button();
            this.btnHex = new Button();
            this.gbSource = new Frame();
            this.rtSourceScrolledWindow = new ScrolledWindow();
            this.rtSource = new TextView();
            this.gbResult = new Frame();
            this.rtResultScrolledWindow = new ScrolledWindow();
            this.rtResult = new TextView();
            this.gbPass = new Frame();
            this.rtPassScrolledWindow = new ScrolledWindow();
            this.rtPass = new TextView();
            this.btnExchange = new Button();
            this.btnComputerInfo = new Button();
            this.btnSnowflake = new Button();
            this.btnJWT = new Button();
            this.rightBox = new VBox();
            this.middleBox = new HBox();
            // 
            // gbFunc
            // 
            this.gbFunc.Add(this.tableFunc);
            this.gbFunc.Name = "gbFunc";
            this.gbFunc.Label = "加密解密";
            //
            // tablrFunc
            //
            this.tableFunc.Attach(this.btnJWT, 1, 2, 14, 15);
            this.tableFunc.Attach(this.btnSnowflake, 0, 1, 14, 15);
            this.tableFunc.Attach(this.btnComputerInfo, 1, 2, 13, 14);
            this.tableFunc.Attach(this.btnTime, 0, 1,    13,14);
            this.tableFunc.Attach(this.btnHtml2, 1, 2   ,12,13);
            this.tableFunc.Attach(this.btnHtml, 0, 1,    12, 13);
            this.tableFunc.Attach(this.btnUrl2, 1, 2,    11,12);
            this.tableFunc.Attach(this.btnUrl, 0, 1,     11,12);
            this.tableFunc.Attach(this.btnDSA2, 1, 2,    10,11);
            this.tableFunc.Attach(this.btnDSA, 0, 1,     10,11);
            this.tableFunc.Attach(this.btnRSA2, 1, 2,    9, 10);
            this.tableFunc.Attach(this.btnRSA, 0, 1,     9, 10);
            this.tableFunc.Attach(this.btnRC42, 1, 2,    8, 9);
            this.tableFunc.Attach(this.btnRC4, 0, 1,     8, 9);
            this.tableFunc.Attach(this.btnAES2, 1, 2,    7, 8);
            this.tableFunc.Attach(this.btnAES, 0, 1,     7, 8);
            this.tableFunc.Attach(this.btnDES2, 1, 2,    6, 7);
            this.tableFunc.Attach(this.btnDES, 0, 1,     6, 7);
            this.tableFunc.Attach(this.btnCRC2, 1, 2,    5, 6);
            this.tableFunc.Attach(this.btnCRC, 0, 1,     5, 6);
            this.tableFunc.Attach(this.btnSHA512, 1, 2,  4, 5);
            this.tableFunc.Attach(this.btnSHA384, 0, 1,  4, 5);
            this.tableFunc.Attach(this.btnSHA256, 1,  2, 3, 4);
            this.tableFunc.Attach(this.btnSHA1, 0, 1,    3, 4);
            this.tableFunc.Attach(this.btnMD52, 1, 2,    2, 3);
            this.tableFunc.Attach(this.btnMD5, 0, 1,     2, 3);
            this.tableFunc.Attach(this.btnB642, 1, 2,    1, 2);
            this.tableFunc.Attach(this.btnB64, 0, 1,     1, 2);
            this.tableFunc.Attach(this.btnHex2, 1, 2,    0, 1);
            this.tableFunc.Attach(this.btnHex, 0, 1,     0, 1);
            this.tableFunc.ColumnSpacing = 4;
            this.tableFunc.RowSpacing = 4;
            // 
            // btnTime
            // 
            this.btnTime.Name = "btnTime";
            this.btnTime.Label = "时间戳";
            this.btnTime.Clicked += new System.EventHandler(this.btnTime_Click);
            // 
            // btnSHA512
            //
            this.btnSHA512.Name = "btnSHA512";
            this.btnSHA512.Label = "SHA512";
            this.btnSHA512.Clicked += new System.EventHandler(this.btnSHA512_Click);
            // 
            // btnSHA384
            //
            this.btnSHA384.Name = "btnSHA384";
            this.btnSHA384.Label = "SHA384";
            this.btnSHA384.Clicked += new System.EventHandler(this.btnSHA384_Click);
            // 
            // btnSHA256
            //
            this.btnSHA256.Name = "btnSHA256";
            this.btnSHA256.Label = "SHA256";
            this.btnSHA256.Clicked += new System.EventHandler(this.btnSHA256_Click);
            // 
            // btnSHA1
            //
            this.btnSHA1.Name = "btnSHA1";
            this.btnSHA1.Label = "SHA1";
            this.btnSHA1.Clicked += new System.EventHandler(this.btnSHA1_Click);
            // 
            // btnHtml2
            //
            this.btnHtml2.Name = "btnHtml2";
            this.btnHtml2.Label = "Html解码";
            this.btnHtml2.Clicked += new System.EventHandler(this.btnHtml2_Click);
            // 
            // btnHtml
            //
            this.btnHtml.Name = "btnHtml";
            this.btnHtml.Label = "Html编码";
            this.btnHtml.Clicked += new System.EventHandler(this.btnHtml_Click);
            // 
            // btnUrl2
            //
            this.btnUrl2.Name = "btnUrl2";
            this.btnUrl2.Label = "Url解码";
            this.btnUrl2.Clicked += new System.EventHandler(this.btnUrl2_Click);
            // 
            // btnUrl
            //
            this.btnUrl.Name = "btnUrl";
            this.btnUrl.Label = "Url编码";
            this.btnUrl.Clicked += new System.EventHandler(this.btnUrl_Click);
            // 
            // btnDSA2
            //
            this.btnDSA2.Name = "btnDSA2";
            this.btnDSA2.Label = "DSA验证";
            this.btnDSA2.Clicked += new System.EventHandler(this.btnDSA2_Click);
            // 
            // btnDSA
            //
            this.btnDSA.Name = "btnDSA";
            this.btnDSA.Label = "DSA签名";
            this.btnDSA.Clicked += new System.EventHandler(this.btnDSA_Click);
            // 
            // btnRSA2
            //
            this.btnRSA2.Name = "btnRSA2";
            this.btnRSA2.Label = "RSA解密";
            this.btnRSA2.Clicked += new System.EventHandler(this.btnRSA2_Click);
            // 
            // btnRSA
            // 
            this.btnRSA.Name = "btnRSA";
            this.btnRSA.Label = "RSA加密";
            this.btnRSA.Clicked += new System.EventHandler(this.btnRSA_Click);
            // 
            // btnRC42
            //
            this.btnRC42.Name = "btnRC42";
            this.btnRC42.Label = "RC4解密";
            this.btnRC42.Clicked += new System.EventHandler(this.btnRC42_Click);
            // 
            // btnRC4
            //
            this.btnRC4.Name = "btnRC4";
            this.btnRC4.Label = "RC4加密";
            this.btnRC4.Clicked += new System.EventHandler(this.btnRC4_Click);
            // 
            // btnAES2
            //
            this.btnAES2.Name = "btnAES2";
            this.btnAES2.Label = "AES解密";
            this.btnAES2.Clicked += new System.EventHandler(this.btnAES2_Click);
            // 
            // btnAES
            //
            this.btnAES.Name = "btnAES";
            this.btnAES.Label = "AES加密";
            this.btnAES.Clicked += new System.EventHandler(this.btnAES_Click);
            // 
            // btnDES2
            //
            this.btnDES2.Name = "btnDES2";
            this.btnDES2.Label = "DES解密";
            this.btnDES2.Clicked += new System.EventHandler(this.btnDES2_Click);
            // 
            // btnDES
            //
            this.btnDES.Name = "btnDES";
            this.btnDES.Label = "DES加密";
            this.btnDES.Clicked += new System.EventHandler(this.btnDES_Click);
            // 
            // btnCRC2
            //
            this.btnCRC2.Name = "btnCRC2";
            this.btnCRC2.Label = "CRC_16";
            this.btnCRC2.Clicked += new System.EventHandler(this.btnCRC2_Click);
            // 
            // btnCRC
            //
            this.btnCRC.Name = "btnCRC";
            this.btnCRC.Label = "CRC_32";
            this.btnCRC.Clicked += new System.EventHandler(this.btnCRC_Click);
            // 
            // btnMD52
            //
            this.btnMD52.Name = "btnMD52";
            this.btnMD52.Label = "MD5_16";
            this.btnMD52.Clicked += new System.EventHandler(this.btnMD52_Click);
            // 
            // btnMD5
            //
            this.btnMD5.Name = "btnMD5";
            this.btnMD5.Label = "MD5_32";
            this.btnMD5.Clicked += new System.EventHandler(this.btnMD5_Click);
            // 
            // btnB642
            //
            this.btnB642.Name = "btnB642";
            this.btnB642.Label = "Base64解码";
            this.btnB642.Clicked += new System.EventHandler(this.btnB642_Click);
            // 
            // btnB64
            //
            this.btnB64.Name = "btnB64";
            this.btnB64.Label = "Base64编码";
            this.btnB64.Clicked += new System.EventHandler(this.btnB64_Click);
            // 
            // btnHex2
            //
            this.btnHex2.Name = "btnHex2";
            this.btnHex2.Label = "HEX解码";
            this.btnHex2.Clicked += new System.EventHandler(this.btnHex2_Click);
            // 
            // btnHex
            //
            this.btnHex.Name = "btnHex";
            this.btnHex.Label = "HEX编码";
            this.btnHex.Clicked += new System.EventHandler(this.btnHex_Click);
            // 
            // gbSource
            //
            this.gbSource.Add(this.rtSourceScrolledWindow);
            this.gbSource.Name = "gbSource";
            this.gbSource.Label = "原文";
            //
            // rtSourceScrolledWindow
            //
            this.rtSourceScrolledWindow.Add(rtSource);
            // 
            // rtSource
            //
            this.rtSource.Name = "rtSource";
            this.rtSource.Buffer.Text = "学无先后达者为师";
            this.rtSource.SizeAllocated += (s, e) => ((TextView)s).ScrollToIter(((TextView)s).Buffer.EndIter, 0, false, 0, 0); ;
            // 
            // gbResult
            //
            this.gbResult.Add(this.rtResultScrolledWindow);
            this.gbResult.Name = "gbResult";
            this.gbResult.Label = "结果";
            //
            // rtResultScrolledWindow
            //
            this.rtResultScrolledWindow.Add(rtResult);
            // 
            // rtResult
            //
            this.rtResult.Name = "rtResult";
            this.rtResult.Buffer.Text = "";
            this.rtResult.SizeAllocated += (s, e) => ((TextView)s).ScrollToIter(((TextView)s).Buffer.EndIter, 0, false, 0, 0); ;
            // 
            // gbPass
            //
            this.gbPass.Add(this.rtPassScrolledWindow);
            this.gbPass.Name = "gbPass";
            this.gbPass.Label = "密码";
            //this.gbPass.HeightRequest = 150;
            //
            // rtPassScrolledWindow
            //
            this.rtPassScrolledWindow.Add(rtPass);
            //this.rtPassScrolledWindow.HeightRequest = 150;
            // 
            // rtPass
            //
            this.rtPass.Name = "rtPass";
            this.rtPass.Buffer.Text = "NewLife";
            this.rtPass.SizeAllocated += (s, e) => ((TextView)s).ScrollToIter(((TextView)s).Buffer.EndIter, 0, false, 0, 0); ;
            // 
            // btnExchange
            //
            this.btnExchange.Name = "btnExchange";
            //this.btnExchange.HeightRequest = 80;
            this.btnExchange.Label = "上下互换";
            this.btnExchange.Clicked += new System.EventHandler(this.btnExchange_Click);
            // 
            // btnComputerInfo
            //
            this.btnComputerInfo.Name = "btnComputerInfo";
            this.btnComputerInfo.Label = "机器信息";
            this.btnComputerInfo.Clicked += new System.EventHandler(this.BtnComputerInfo_Click);
            // 
            // btnSnowflake
            //
            this.btnSnowflake.Name = "btnSnowflake";
            this.btnSnowflake.Label = "雪花Id";
            this.btnSnowflake.Clicked += new System.EventHandler(this.btnSnowflake_Click);
            // 
            // btnJWT
            //
            this.btnJWT.Name = "btnJWT";
            this.btnJWT.Label = "JWT令牌";
            this.btnJWT.Clicked += new System.EventHandler(this.btnJWT_Click);
            //
            // middleBox
            //
            this.middleBox.PackEnd(this.btnExchange, false, false, 2);
            this.middleBox.PackEnd(this.gbPass, true, true, 2);
            //
            // rightBox
            //
            this.rightBox.PackStart(this.gbSource, true, true, 2);
            this.rightBox.PackStart(this.middleBox, false, false, 2);
            this.rightBox.PackStart(this.gbResult, true, true, 2);
            // 
            // FrmSecurity
            //
            this.PackStart(this.gbFunc, false, false, 4);
            this.PackStart(this.rightBox, true, true, 4);
            this.Name = "FrmSecurity";
            this.Margin = 4;
            //this.Label = "加密解密";
        }

        #endregion

        private Frame gbFunc;
        private Table tableFunc;
        private Frame gbSource;
        private ScrolledWindow rtSourceScrolledWindow;
        private TextView rtSource;
        private Frame gbResult;
        private ScrolledWindow rtResultScrolledWindow;
        private TextView rtResult;
        private Button btnHex2;
        private Button btnHex;
        private Button btnB642;
        private Button btnB64;
        private Button btnMD52;
        private Button btnMD5;
        private Button btnCRC2;
        private Button btnCRC;
        private Button btnHtml2;
        private Button btnHtml;
        private Button btnUrl2;
        private Button btnUrl;
        private Button btnDSA2;
        private Button btnDSA;
        private Button btnRSA2;
        private Button btnRSA;
        private Button btnRC42;
        private Button btnRC4;
        private Button btnAES2;
        private Button btnAES;
        private Button btnDES2;
        private Button btnDES;
        private Frame gbPass;
        private ScrolledWindow rtPassScrolledWindow;
        private TextView rtPass;
        private Button btnExchange;
        private Button btnSHA512;
        private Button btnSHA384;
        private Button btnSHA256;
        private Button btnSHA1;
        private Button btnTime;
        private Button btnComputerInfo;
        //private CheckBox cbBase64;
        //private CheckBox cbHex;
        //private CheckBox cbString;
        private RadioButton rbBase64;
        private RadioButton rbHex;
        private RadioButton rbString;
        private RadioButton rbBase642;
        private RadioButton rbHex2;
        private RadioButton rbString2;
        private Button btnJWT;
        private Button btnSnowflake;
        private VBox rightBox;
        private HBox middleBox;
    }
}