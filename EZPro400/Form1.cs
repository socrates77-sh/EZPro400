using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Xml;
namespace EZPro400
{
    public partial class Form1 : Form
    {
        public static byte[] HexSourceBuffer = new byte[0x10000];
        public static byte[] HexChipBuffer = new byte[0x10000];
        ArrayList Xml_File = new ArrayList();
        ArrayList Xml_Mcu = new ArrayList();
        mcu_class mcu_tyep = new mcu_class();             //mcu类型

        public static MCU_Cfg MCUCFG;

        public Form1()
        {
            InitializeComponent();
            find_mcu_file();
            get_mcu_type();
            show_tool(Xml_Mcu); //根据配置文件更新生成界面
            ushort result =  0xffff;
            for (int i = 0; i < 0x8000; i += 2)
            {
                HexSourceBuffer[i] = (byte)(result % 0x100);
                HexSourceBuffer[i + 1] = (byte)(result / 0x100);
            }

            for (int i = 0; i < 0x8000; i += 2)
            {
                HexChipBuffer[i] = (byte)(result % 0x100);
                HexChipBuffer[i + 1] = (byte)(result / 0x100);
            }
            show_soure_buffer();
        }
        private void show_soure_buffer()
        {
            int line = HexSourceBuffer.Length/16;
            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();
            for (int i = 0; i < line; i++)
            {
                int index = this.dataGridView1.Rows.Add();
                for (int j = 0; j < 16;j++ )
                {
                    this.dataGridView1.Rows[index].Cells[j].Value = string.Format("{0:x2}", HexSourceBuffer[i*16+j]);
                }
                   
                
            }
        }
        private void read_file_buffer(string file_d)
        {

        }
       private void find_mcu_file()
        {
            DirectoryInfo directory = new DirectoryInfo("opt\\");
            FileInfo[] files = directory.GetFiles("*.xml");
            //输出文件个数
            //Console.WriteLine("Files Number:{0}", files.Length);
            //遍历文件
            foreach (FileInfo file in files)
            {
                Xml_File.Add(file.Directory+"\\"+file.Name);
                //Console.WriteLine(file.Name);
            }
            //Console.ReadKey();
        }

        private void get_mcu_type()
       {
           string file_list;
           int i=0;
           int xml_size = 0;
            XmlNode MCU;
            XmlNode next; 
           i = Xml_File.Count;
           XmlDocument doc = new XmlDocument();
            for(i=0;i<Xml_File.Count;i++)
            {
                file_list = Xml_File[i] as string;
                XmlDocument xmlDoc = new XmlDocument();
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreComments = true;
                XmlReader reader = XmlReader.Create(file_list, settings);
                xmlDoc.Load(reader);
                reader.Close();
                string temp;
                ArrayList Xml_OPTION_Name = new ArrayList();

                ArrayList Xml_OPTION_Value = new ArrayList();
                ArrayList xml_option = new ArrayList();
                XmlNode next2;
                XmlNode next3;
                XmlNode next4;
                
                int j = 0;

                try
                {
                     
                    MCU = xmlDoc.SelectSingleNode("MCU");
                    //xml_size = MCU.NextSibling;
                    //mcu_tyep.CHIP_TYPE = (MCU.SelectSingleNode("/CHIP_TYPE")).InnerText;
                    next = MCU.SelectSingleNode("CHIP_TYPE");
                    
                   // UInt16 as1=StrToHex("0x001");
                   
                    mcu_tyep.CHIP_TYPE = next.InnerText;
                    mcu_tyep.CHIP_NAME = MCU.SelectSingleNode("CHIP_NAME").InnerText;
                    mcu_tyep.CHIP_ID =StrToHex( MCU.SelectSingleNode("CHIP_ID").InnerText);
                    mcu_tyep.ID_ADDR =StrToHex( MCU.SelectSingleNode("ID_ADDR").InnerText);
                    mcu_tyep.USER_ROM_START = StrToHex(MCU.SelectSingleNode("USER_ROM_START").InnerText);
                    mcu_tyep.USER_ROM_SIZE = StrToHex(MCU.SelectSingleNode("USER_ROM_SIZE").InnerText);
                    mcu_tyep.CS_ADDR = StrToHex(MCU.SelectSingleNode("CS_ADDR").InnerText);
                    mcu_tyep.CHIP_BIT = StrToHex(MCU.SelectSingleNode("CHIP_BIT").InnerText);
                   // UInt16.Parse
                    mcu_tyep.VDD = StrToHex(MCU.SelectSingleNode("VDD").InnerText);
                    mcu_tyep.VPP = StrToHex(MCU.SelectSingleNode("VPP").InnerText);
                    mcu_tyep.opbit = StrToHex(MCU.SelectSingleNode("opbit").InnerText);

                    next = MCU.SelectSingleNode("OPTION");
                    
                    for ( j= 0; j < next.ChildNodes.Count; j++)
                    {
                         Xml_OPTION_Name.Add( next.ChildNodes[j].Name);
                         next2 = next.SelectSingleNode(next.ChildNodes[j].Name);
                         writer_data mcu_writer_data = new writer_data();  //烧录配置
                         mcu_writer_data.name = next.ChildNodes[j].Name;
                         mcu_writer_data.addr = StrToHex((next2.SelectSingleNode("address").InnerText));
                         mcu_writer_data.Annotation.Clear();
                         
                         string config_name;
                         int start_bit = 0;
                         int end_bit = 0;
                         int bit_size = 0;
                         //获取配置信息
                        for(int k = 1;k<next2.ChildNodes.Count;k++)
                        {
                            string[] config_bit = new string[50];
                            string[] config_value = new string[50];
                            Xml_OPTION_Value.Clear();
                            Xml_OPTION_Value.Add(next2.ChildNodes[k].Name);
                            config_name = next2.ChildNodes[k].Name;
                            next3 = next2.SelectSingleNode(next2.ChildNodes[k].Name);
                             
                            for(int l = 2 ;l<next3.ChildNodes.Count;l++)
                            {
                                Xml_OPTION_Value.Add(next3.ChildNodes[l].Name);
                                config_bit[l-2] = next3.ChildNodes[l].Name.Remove(0,1);
                                config_value[l - 2] = next3.SelectSingleNode(next3.ChildNodes[l].Name).InnerText;
                            }
                            string bit = next3.SelectSingleNode(next3.ChildNodes[1].Name).InnerText;
                            int str_1 = bit.IndexOf('[');
                            int str_2 = bit.IndexOf(':');
                            int str_3 = bit.IndexOf(']');

                            if(str_2 < 0)
                            {
                                start_bit = int.Parse(bit.Substring(str_1 + 1, str_3 - str_1 - 1));
                                end_bit = 0;
                                bit_size = 1;
                            }
                            else
                            {
                                start_bit = int.Parse(bit.Substring(str_2+1, str_3 - str_2-1));
                                end_bit = int.Parse(bit.Substring(str_1 + 1, str_2 - str_1 - 1));
                                bit_size = end_bit - start_bit+1;
                            }
                            mcu_writer_data.Annotation.Add(config_name);
                            mcu_writer_data.Annotation.Add(start_bit);
                            mcu_writer_data.Annotation.Add(end_bit);
                            mcu_writer_data.Annotation.Add(bit_size);
                            mcu_writer_data.Annotation.Add(config_bit);
                            mcu_writer_data.Annotation.Add(config_value);
                        }
                        mcu_tyep.OPTION.Add(mcu_writer_data);
                        //xml_option.Add(mcu_writer_data);
                        
                        //xml_option.AddRange();
                    }
                    
                   /* */
                }
                catch (Exception e)
                {
                    MessageBox.Show(null, "程序文件出错请重新安装。\n\n这个程序即将退出。", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //throw e;
                    Application.Exit();//退出程序
                }
                Xml_Mcu.Add(mcu_tyep);
            }
            
       }
        private void show_tool(ArrayList show_data)
        {
             mcu_class show_mcu;
             writer_data show_writer;
             int option_size;
             for(int i=0;i<show_data.Count;i++)
             {
                 show_mcu = show_data[i] as mcu_class;
                 Chip_name.Items.Add(show_mcu.CHIP_NAME);
             }

             Chip_name.SelectedIndex = 0;
             show_mcu = show_data[0] as mcu_class;
             option_size = show_mcu.OPTION.Count;
             show_writer = show_mcu.OPTION[0] as writer_data;
               
             if(option_size==0)
             {

             }
             else if(option_size == 1)
             {

             }
             else if(option_size == 2)
             {

             }
             else if(option_size == 3)
             {

             }
             else if(option_size == 4)
             {
                   op1.Visible = true;
                   op2.Visible = true;
                   op3.Visible = true;
                   op4.Visible = true;
                   op5.Visible = false;

                   top1.Visible = true;
                   top2.Visible = true;
                   top3.Visible = true;
                   top4.Visible = true;
                   top5.Visible = false;

                   
             }
             else if(option_size == 5)
             {
                   Label[] lab = new Label[5];
                   TextBox[] txb = new TextBox[5];
                   Label[] c = new Label[19];
                   ComboBox[] co = new ComboBox[19];
                   op1.Visible = true;
                   op2.Visible = true;
                   op3.Visible = true;
                   op4.Visible = true;
                   op5.Visible = true;

                   top1.Visible = true;
                   top2.Visible = true;
                   top3.Visible = true;
                   top4.Visible = true;
                   top5.Visible = true;
                   lab[0] = op1; 
                   lab[1] = op2; 
                   lab[2] = op3;
                   lab[3] = op4;
                   lab[4] = op5;
                   txb[0] = top1; txb[1] = top2; txb[2] = top3; txb[3] = top4; txb[4] = top5;
                   c[0] = c1; c[1] = c2; c[2] = c3; c[3] = c4; c[4] = c5; c[5] = c6; c[6] = c7; c[7] = c8; c[8] = c9; c[9] = c10; c[10] = c11;
                   c[11] = c12; c[12] = c13; c[13] = c14; c[14] = c15; c[15] = c16; c[16] = c17; c[17] = c18; c[18] = c19;

                   co[0] = co1; co[1] = co2; co[2] = co3; co[3] = co4; co[4] = co5; co[5] = co6; co[6] = co7; co[7] = co8; co[8] = co9; co[10] = co11; co[9] = co10;
                   co[11] = co12; co[12] = co13; co[13] = co14; co[14] = co15; co[15] = co16; co[16] = co17; co[17] = co18; co[18] = co19;
                   int j = 0;
                   for(int i = 0;i<option_size;i++)
                   {
                       show_writer = show_mcu.OPTION[i] as writer_data;
                       lab[i].Text = show_writer.name;
                       txb[i].Text = ConvertHex(Convert.ToString((int)show_writer.data));
                       for(int k=0;k<((show_writer.Annotation.Count+1)/6);k++)
                       {
                           string a = show_writer.Annotation[k * 6] as string;
                           if (a == "Fix_High")
                           {

                           }
                           else if (a == "default")
                           {

                           }
                           else if (a == "CLKSEL")
                           {

                           }
                           else if(a == "ROTP")
                           {

                           }
                           else if(a == "FDS")
                           {
                           
                           }
                           else if(a == "RCSMTB")
                           { }
                           else
                           {
                               c[j].Text = show_writer.Annotation[k * 6] as string;
                               c[j].Visible = true;
                               string[] config_bit = new string[50];
                               string[] config_value = new string[50];
                               config_bit = show_writer.Annotation[k * 6+4] as string[];
                               config_value = show_writer.Annotation[k * 6 + 5] as string[];
                               int cnt=0;
                               for (int ia = 0; ia < 50;ia++ )
                               {
                                   if (config_value[ia]==null)
                                   {
                                       break;
                                   }
                                   cnt++;
                               }
                               for(int ib=0;ib<cnt;ib++)
                               {
                                   string temp = config_bit[ib] + ":" + config_value[ib];
                                   co[j].Items.Add(temp);
                                   co[j].Visible = true;
                                   co[j].SelectedIndex = 0;
                               }
                                   j++;
                           }
                       }
                       
                   }
                  op1 = lab[0]  ;
                  op2 = lab[1]  ;
                  op3 = lab[2]  ;
                  op4 = lab[3]  ;
                  op5 = lab[4]  ;
                  top1 = txb[0]; top2 = txb[1]; top3 = txb[2]; top4 = txb[3]; top5 = txb[4];
                  c1 = c[0]; c2 = c[1]; c3 = c[2]; c4 = c[3]; c5 = c[4]; c6 = c[5]; c7 = c[6]; c8 = c[7];
                  c9 = c[8]; c10 = c[9]; c11 = c[10]; c12 = c[11]; c13 = c[12]; c14 = c[13]; c15 = c[14]; c16 = c[15]; c17 = c[16]; c18 = c[17];
                  c19 = c[18];

                  co1 = co[0]; co2 = co[1]; co3 = co[2]; co4 = co[3]; co5 = co[4]; co6 = co[5]; co7 = co[6]; co8 = co[7]; co9 = co[8]; co10 = co[9];
                  co11 = co[10]; co12 = co[11]; co13 = co[12]; co14 = co[13]; co15 = co[14]; co16 = co[15]; co17 = co[16]; co18 = co[17];
                  co19 = co[18];
             }

             
        }
        public static string GetHexChar(string value)
        {
            string sReturn = string.Empty;
            switch (value)
            {
                case "10":
                    sReturn = "A";
                    break;
                case "11":
                    sReturn = "B";
                    break;
                case "12":
                    sReturn = "C";
                    break;
                case "13":
                    sReturn = "D";
                    break;
                case "14":
                    sReturn = "E";
                    break;
                case "15":
                    sReturn = "F";
                    break;
                default:
                    sReturn = value;
                    break;
            }
            return sReturn;
        }
        public static string ConvertHex(string value)
        {
            string sReturn = string.Empty;
            try
            {

                while (ulong.Parse(value) >= 16)
                {
                    ulong v = ulong.Parse(value);
                    sReturn = GetHexChar((v % 16).ToString()) + sReturn;
                    value = Math.Floor(Convert.ToDouble(v / 16)).ToString();
                }
                sReturn = GetHexChar(value) + sReturn;
            }
            catch
            {
                sReturn = "###Valid Value!###";
            }
            return "0x"+sReturn;
        }  

        private UInt16 StrToHex(string st)
        {
            UInt16 returnt=0x0000;
            int st_leght;
            string temp="";
            st_leght = st.Length;
            if(st_leght == 0)
            {
                returnt = 0;

            }
            else if(st_leght==1)
            {
                returnt = UInt16.Parse(st);
            }
            else if(st_leght == 2)
            {
                returnt = UInt16.Parse(st);
            }
            else
            {
                temp = st.Substring(1, 1);
                if(temp=="x")
                {
                    temp = st.Substring(2, st.Length-2);
                    returnt = UInt16.Parse(temp, System.Globalization.NumberStyles.AllowHexSpecifier);
                }
                else if(temp=="X")
                {
                    temp = st.Substring(2, st.Length-2);
                    returnt = UInt16.Parse(temp, System.Globalization.NumberStyles.AllowHexSpecifier);
                }
                else
                {
                    returnt = UInt16.Parse(st);
                }
            }


            return returnt;
        }
        private void toolStripTextBox1_Click(object sender, EventArgs e)
        {

        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void toolStripLabel1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.sinomcu.com"); 
        }

        private void 打开ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "c:\\";//注意这里写路径时要用c:\\而不是c:\
            openFileDialog.Filter = "烧录文件|*.s19|烧录文件|*.wrt";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                //textBox1.Text = openFileDialog.FileName;
                //File fileOpen=new File(textBox1.Text);
                //isFileHaveName=true;
                //richTextBox1.Text=fileOpen.ReadFile();
                //richTextBox1.AppendText("");

            }
        }
        private void open_s91_file()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "c:\\";//注意这里写路径时要用c:\\而不是c:\
            openFileDialog.Filter = "烧录文件|*.s19|烧录文件|*.wrt";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string read_start;
                FileInfo fi = new FileInfo(openFileDialog.FileName);
                StreamReader sr = fi.OpenText();
                int i=0;

                string type;
                string count;
                string address;
                string data;
                string checksum;

                while (sr.Peek() > 0)
                {
                    i++;
                    read_start=sr.ReadLine();
                    //int index = this.dataGridView1.Rows.Add();
                   // this.dataGridView1.Rows[index].Cells[0].Value = "1"+i ;
                   // this.dataGridView1.Rows[index].Cells[1].Value = "2"+i;
                   // this.dataGridView1.Rows[index].Cells[2].Value = "监听";
                }
                

                 

                //read_start
                //textBox1.Text = openFileDialog.FileName;
                //File fileOpen=new File(textBox1.Text);
                //isFileHaveName=true;
                //richTextBox1.Text=fileOpen.ReadFile();
                //richTextBox1.AppendText("");

            }
        }
        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            open_s91_file();
        }

        private void 保存ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
           
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void usb_OnDataRecieved(object sender, UsbLibrary.DataRecievedEventArgs args)
        {

        }

        private void usb_OnDataSend(object sender, EventArgs e)
        {

        }

        private void usb_OnDeviceArrived(object sender, EventArgs e)
        {

        }

        private void usb_OnDeviceRemoved(object sender, EventArgs e)
        {

        }

        private void usb_OnSpecifiedDeviceArrived(object sender, EventArgs e)
        {

        }

        private void usb_OnSpecifiedDeviceRemoved(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void dataGridView1_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            e.Row.HeaderCell.Value = "0x"+string.Format("{0:x4}", e.Row.Index * 16); 
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView2_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            e.Row.HeaderCell.Value = "0x" + string.Format("{0:x4}", e.Row.Index * 16); 
        }
    }
}
