using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Collections;

namespace EZPro400
{
    public class mcu_class
    {
        public string CHIP_TYPE;
        public string CHIP_NAME;
        public UInt16 CHIP_ID;
        public UInt16 ID_ADDR;
        public UInt16 USER_ROM_START;
        public UInt16 USER_ROM_SIZE;
        public UInt16 CS_ADDR;
        public UInt16 CHIP_BIT;
        public UInt16 VDD;
        public UInt16 VPP;
        public ArrayList OPTION = new ArrayList();
        public UInt16 opbit;
    }
    public struct MCU_Cfg
    {
        public int id;
        public int id_Addr;
        public int romsize;
        public int romStart_Addr;
        public int opbit;
        public int cs_addr;
        public string optValueShow;
        public ArrayList OPTIONList;// 存储各OPBIT
        public ArrayList LinkValueList;// 存储各联动项
    }
     public class writer_data
     {
         public UInt16 addr;
         public UInt16 data;
         public string name;
         public ArrayList Annotation = new ArrayList();


     }


}
