using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;

namespace Simulator
{
    public class TrackObjectRecord
    {
        public GeoCoordinate MessageCoordinate;
        public double Distance;
        public byte[] TReceptionDataExBytes;

        public TrackObjectRecord(GeoCoordinate MessageCoordinate, double Distance, byte[] TReceptionDataExBytes)
        {
            this.MessageCoordinate = MessageCoordinate;
            this.Distance = Distance;
            this.TReceptionDataExBytes = TReceptionDataExBytes;
        }
    }

    public enum TargetDistanceSource
    {
        MCO,
        SAUT
    }
    public class TargetDistance
    {
        TargetDistanceSource distanceSource;
        public double trackDistance;
        public int targetDistance;

        public TargetDistance(double trackDistance, int targetDistances)
        {
            this.trackDistance = trackDistance;
            this.targetDistance = targetDistances;
        }

        public TargetDistance(string[] message, double trackDistance)
        {
            this.trackDistance = trackDistance;
            this.targetDistance = GetTargetDistance(message);
        }

        private int GetTargetDistance(string[] message)
        {
            if (message[1].Contains("0A08"))    // MCO_STATE_A
            {
                this.distanceSource = TargetDistanceSource.MCO;
                return (((Convert.ToInt32(message[5], 16)) & 31) << 8) + (Convert.ToInt32(message[6], 16));
            }
            if (message[1].Contains("39E7"))    // SAUT_STATE_A
            {
                this.distanceSource = TargetDistanceSource.SAUT;
                return ((Convert.ToInt32(message[6], 16)) << 8) + ((Convert.ToInt32(message[7], 16)) >> 7);
            }
            else
            {
                throw new Exception();
            }
        }

        public override string ToString()
        {
            return this.trackDistance.ToString().Replace(',', '.') + "|" + this.targetDistance.ToString();
        }

        public static void LoadFromFile(string filename, ref List<TargetDistance> mcoTargets, ref List<TargetDistance> sautTargets)
        {
            string line = null;
            StreamReader streamReader = new StreamReader(filename);
            string[] data;
            bool loadMco = true;
            while ((line = streamReader.ReadLine()) != null)
            {
                if (line == "-")
                {
                    loadMco = false;
                    continue;
                }
                if (loadMco)
                {
                    data = line.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    mcoTargets.Add(new TargetDistance(Double.Parse(data[0], CultureInfo.InvariantCulture), Int32.Parse(data[1])));
                }
                else
                {
                    data = line.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    sautTargets.Add(new TargetDistance(Double.Parse(data[0], CultureInfo.InvariantCulture), Int32.Parse(data[1])));
                }
            }
        }
    }



    //Формат телеграммы ОФМ
    //  0 - САУТ-ЦМ (код Голея)
    //  1 - САУТ-Ц
    //  2 - САУТ-ЦМ (код Рао-Редди)
    public enum TelegramFormat : byte
    {
        SautCm_Golay,
        SautC,
        SautCm_RaoReddy
    }

    public enum TTLType : byte
    {
        tlOut,
        tlBeforeIn,
        tlIn,
        tlRoute,
        tlSimple
    }

    public class TReceptionDataEx
    {
        public byte[] data = new byte[32];


        private void UshortToBytes(ushort value, ref byte[] data, int offset)
        {
            data[offset] = (byte)(value & 0xFF);
            data[offset + 1] = (byte)((value >> 8) & 0xFF);
        }
        private ushort UshortFromBytes(byte[] data, int offset)
        {
            return (ushort)(((int)data[offset + 1] << 8) + (int)data[offset]);
        }
        private void DoubleToBytes(double value, ref byte[] data, int offset)
        {
            byte[] valueBytes = BitConverter.GetBytes(value);
            Buffer.BlockCopy(valueBytes, 0, data, offset, valueBytes.Length);
        }
        private double DoubleFromBytes(byte[] data, int offset)
        {
            return BitConverter.ToDouble(data, offset);
        }
        private void BooleanToBytes(bool value, ref byte[] data, int offset)
        {
            byte[] valueBytes = BitConverter.GetBytes(value);
            Buffer.BlockCopy(valueBytes, 0, data, offset, valueBytes.Length);
        }
        private bool BooleanFromBytes(byte[] data, int offset)
        {
            return BitConverter.ToBoolean(data, offset);
        }


        //L a-d в метрах x 100 (L б/у)
        public ushort FRD_Length_1_4
        {
            get
            {
                return UshortFromBytes(data, 0);
            }
            set
            {
                UshortToBytes(value, ref data, 0);
            }
        }
        //Диаметр бандажа в мм
        public ushort FRD_Calibre
        {
            get
            {
                return UshortFromBytes(data, 2);
            }
            set
            {
                UshortToBytes(value, ref data, 2);
            }
        }
        //Несущая частота
        public double FRD_Frec
        {
            get
            {
                return DoubleFromBytes(data, 4);
            }
            set
            {
                DoubleToBytes(value, ref data, 4);
            }
        }
        //Ток шлейфа
        public double FRD_I
        {
            get
            {
                return DoubleFromBytes(data, 12);
            }
            set
            {
                DoubleToBytes(value, ref data, 12);
            }
        }
        //Кодированный прием?
        public bool FRD_OFM
        {
            get
            {
                return BooleanFromBytes(data, 20);
            }
            set
            {
                BooleanToBytes(value, ref data, 20);
            }
        }

        //Тип генератора:
        //Голей: 11 – Вых. 1-й зоны, 00 – Вых. 2-й зоны, 10 – Пр.Вх., 01 – Вх./Марш.
        //Рао-Редди: 11 – Вых., 10 – Пр.Вх., 01 – Вх., 00 - Марш.
        public TTLType FRD_Type
        {
            get
            {
                return (TTLType)data[21];
            }
            set
            {
                //UshortToBytes(value, ref data, 23);
                data[21] = (byte)value;
            }
        }
        //Код генератора (номер перегона)
        //Голей: 1..8191 (Вх./Марш. - 1..2047, 4097..6143)
        //Рао-Редди: (1..8191)
        public ushort FRD_Code
        {
            get
            {
                return UshortFromBytes(data, 22);
            }
            set
            {
                UshortToBytes(value, ref data, 22);
            }
        }
        //Номер генератора
        //Голей: (0..15)
        //Рао-Редди: (0..31)
        public byte FRD_Number
        {
            get
            {
                return data[24];
            }
            set
            {
                data[24] = value;
            }
        }
        //Номер маршрута
        public byte FRD_Route
        {
            get
            {
                return data[25];
            }
            set
            {
                data[25] = value;
            }
        }
        //Формат телеграммы ОФМ
        //  0 - САУТ-ЦМ (код Голея)
        //  1 - САУТ-Ц
        //  2 - САУТ-ЦМ (код Рао-Редди)
        public byte FRD_FormatOFM
        {
            get
            {
                return data[26];
            }
            set
            {
                data[26] = value;
            }
        }

        //Код Рао-Редди...
        //Номер (тип) сообщения (0..7)
        public byte FRD_MesNum
        {
            get
            {
                return data[27];
            }
            set
            {
                data[27] = value;
            }
        }
        //Номер маршрута 2 (0..15)
        public byte FRD_Route2
        {
            get
            {
                return data[28];
            }
            set
            {
                data[28] = value;
            }
        }
        //Количество свободных б/у до светофора с заданным маршрутом (0..15)
        public byte FRD_FreeBU
        {
            get
            {
                return data[29];
            }
            set
            {
                data[29] = value;
            }
        }
        //Признак места установки генератора (True - до изостыка)
        public bool FRD_IS
        {
            get
            {
                return BooleanFromBytes(data, 30);
            }
            set
            {
                BooleanToBytes(value, ref data, 30);
            }
        }
        //!!!
        public bool FRD_Ext
        {
            get
            {
                return BooleanFromBytes(data, 31);
            }
            set
            {
                BooleanToBytes(value, ref data, 31);
            }
        }

        //Включать 27 КГц?
        public bool FRD_27
        {
            get
            {
                return BooleanFromBytes(data, 21);
            }
            set
            {
                BooleanToBytes(value, ref data, 21);
            }
        }
        //L a-b в метрах x 100 (профиль)
        public ushort FRD_Length_1_2
        {
            get
            {
                return UshortFromBytes(data, 22);
            }
            set
            {
                UshortToBytes(value, ref data, 22);
            }
        }
        //L b-c в метрах x 100 (Vo или S2п)
        public ushort FRD_Length_2_3
        {
            get
            {
                return UshortFromBytes(data, 24);
            }
            set
            {
                UshortToBytes(value, ref data, 24);
            }
        }
    }
}
