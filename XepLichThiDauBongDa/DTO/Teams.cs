using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XepLichThiDauBongDa.DTO
{
    public class Teams
    {
        private string id;
        private string name;
        private string abbreviation;
        private byte[] logo;

        public Teams(string id, string name, string abbreviation, byte[] logo)
        {
            this.id = id;
            this.name = name;
            this.abbreviation = abbreviation;
            this.logo = logo;
        }

        public Teams(DataRow row)
        {
            this.id = row["TeamID"].ToString();
            this.name = row["TeamName"].ToString();
            this.abbreviation = row["Abbreviation"].ToString();
            if(row["Logo"] != DBNull.Value)
            {
                this.logo = (byte[])row["Logo"];
            }
        }

        public string Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public string Abbreviation { get => abbreviation; set => abbreviation = value; }
        public byte[] Logo { get => logo; set => logo = value; }
    }
}
