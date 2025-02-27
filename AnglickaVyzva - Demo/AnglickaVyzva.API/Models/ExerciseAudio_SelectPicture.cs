﻿using AnglickaVyzva.API.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnglickaVyzva.API.Models
{
    public class ExerciseAudio_SelectPicture : IExercise
    {
        public int Order { get; set; }
        public int Points { get; set; } = 1;
        public string NameCZ { get; set; }
        public string NameEN { get; set; }
        public bool HasError { get; set; }
        public string Type { get; set; } = "audio_select_picture";
        public bool IsLock { get; set; }
        public bool IsDone { get; set; }

        public string Title { get; set; }
        public string Subtitle { get; set; }
        public List<ItemAudio_SelectPicture> ItemList { get; set; }


        public class ItemAudio_SelectPicture
        {
            public int ID { get; set; }
            public string Audio { get; set; }
            public string Translation { get; set; }
            public List<string> Images { get; set; } //Ten prvni je zaroven spravna odpoved
        }

        /// <summary>
        /// Ozve se zvuk a podle nej se vybere jeden ze ctyr obrazku
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="section"></param>
        /// <param name="includeLocked"></param>
        public ExerciseAudio_SelectPicture(DataTable sheet, Section section, bool includeLocked)
        {
            if (sheet.Rows[1].ItemArray[0] != DBNull.Value)
            {
                try
                {
                    Points = Convert.ToInt32(sheet.Rows[1].ItemArray[0]);
                }
                catch
                {

                }
            }

            ItemList = new List<ItemAudio_SelectPicture>();

            NameCZ = (string)sheet.Rows[0].ItemArray[0];

            // Obsahuje podnadpis
            if (NameCZ.Contains("~"))
            {
                var parts = NameCZ.Split('~');
                NameCZ = parts[0];
                Subtitle = parts[1];
            }

            Title = NameCZ;

            if (includeLocked == false)
                IsLock = sheet.TableName.Contains("{P}") ? true : false;

            if (IsLock)
                return;

            for (int i = 2; i < sheet.Rows.Count; i++)
            {
                DataRow row = sheet.Rows[i];
                ItemAudio_SelectPicture item = new ItemAudio_SelectPicture();
                item.Images = new List<string>();

                if (row.ItemArray.Length < 4)
                {
                    HasError = true;
                    return;
                }

                if (i > 3 && row.ItemArray[1] == DBNull.Value)
                {
                    break;
                }

                item.ID = Convert.ToInt32(row.ItemArray[0]);

                item.Audio = row.ItemArray[1] == System.DBNull.Value ? "" : (string)row.ItemArray[1];
                item.Audio = AudioHelper.CreateAudio(item.Audio);
                
                for (int col = 2; col < 6; col++)
                {
                    if (row.ItemArray[col] == DBNull.Value)
                        break;

                    string image =  ImageHelper.CreateImage(row.ItemArray[col], section);
                    item.Images.Add(image);
                }

                item.Translation = row.ItemArray[6] == DBNull.Value ? "" : Convert.ToString(row.ItemArray[6]).Trim();

                ItemList.Add(item);
            }
        }
    }
}
