using AEditor;
using System.Collections.Generic;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;
using String = System.String;

namespace Avatar_Editor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        int CurrentIndex = 0;
        public class AvatarData
        {
            public Int32 ItemNo;
            public Int32 ImgNo;
            public Int32 IsNew;
            public string Name;
            public string imgNum;
            public byte[] imgNumWC;
            public byte[] menuId;
            public byte Visible; //Visible
            public byte Unk03;
            public byte[] Unk01;
            public byte HabilitarSemana; //Semana
            public byte Unk04;
            public byte Unk05;
            public byte[] Unk02;
            public Int32 PriceByGoldForW;
            public Int32 PriceByCashForW;
            public Int32 abilitar2Horas; //
            public Int32 PriceByGoldForH; //
            public Int32 PriceByCashForH; //
            public Int32 HabilitarMes; //Mes
            public Int32 PriceByGoldForM;
            public Int32 PriceByCashForM;
            public Int32 HabilitarDia; //
            public Int32 PriceByGoldForD; //
            public Int32 PriceByCashForD; //
            public Int32 HabilitarIlimitado; //Etherno
            public Int32 PriceByGoldForI;
            public Int32 PriceByCashForI;
            public byte HabilitarGold; //Activacion Gold
            public byte HabilitarCash; //Activacion Cash
            public Int32 Habilitar2Horas;
            public byte Unk12;
            public byte Unk13;
            public Int32 Delay;
            public Int32 Pit_Angle;
            public Int32 Attack;
            public Int32 Defence;
            public Int32 Energy;
            public Int32 ItemSkipDelay;
            public Int32 Shield_Recovery;
            public Int32 Popularity;
            public string Description;
            public byte[] RemainData1;
            public byte[] RemainData2;
            public byte[] RemainData3;
            public byte[] RemainData4;
            public int codigoAv;
            public byte[] CRC;
            public byte[] CRCInicial;
            public byte[] RemainData;
        }

        MemoryStream FileStream = null;
        List<AvatarData> Temp = new List<AvatarData>();
       
        int numAvatar = 0;
        byte[] numAvatar2 = null;
        string nomeArquivo = "";
        

        //Função para decriptar os arquivos MH,MB,MF e etc (é necessário criar a função para abrir ex-item e s3)
        public void DecriptarWC(string caminho)
        {
            if (caminho != null)
            {


                byte[] avatar = File.ReadAllBytes(caminho);

                // Leia os primeiros 4 bytes
                StreamDataReader AV = new StreamDataReader(avatar);
                numAvatar = AV.ReadInt32(); // Isso lê os primeiros 4 bytes
                numAvatar2 = BitConverter.GetBytes(numAvatar);//Isso adiciona o mesmo valor acima, mas pra usar na encriptação depois
                using (FileStream fs = new FileStream("Temp/AvatarDecriptado.dat", FileMode.Create, FileAccess.Write))
                {
                    // Escreve os 4 primeiros bytes de volta no início do arquivo
                    byte[] numAvatarBytes = BitConverter.GetBytes(numAvatar);
                    fs.Write(numAvatarBytes, 0, numAvatarBytes.Length); // Escreve os 4 bytes

                    int i = 0;
                    while (i < numAvatar)
                    {
                        // Lê os próximos dados
                        byte[] CRCInicial = AV.ReadBytes(3); // Lê os 3 bytes do CRC
                        byte[] AvatarDecrypted = GBCrypto.Decompress(AV.ReadBytes(256), 644); // Decripta os dados
                        StreamDataReader AV2 = new StreamDataReader(AvatarDecrypted);
                        AvatarData T = new AvatarData();

                        // Leitura normal dos campos
                        T.CRCInicial = CRCInicial;
                        T.ItemNo = AV2.ReadInt32();
                        T.ImgNo = AV2.ReadInt32();
                        T.IsNew = AV2.ReadInt32();
                        T.Name = AV2.ReadPStringFixed(23);
                        T.Visible = AV2.ReadByte(); //Visible
                        T.Unk03 = AV2.ReadByte();
                        T.HabilitarSemana = AV2.ReadByte(); //Semana
                        T.Unk04 = AV2.ReadByte();
                        T.Unk05 = AV2.ReadByte();
                        T.PriceByGoldForW = AV2.ReadInt32();
                        T.PriceByCashForW = AV2.ReadInt32();
                        T.Habilitar2Horas = AV2.ReadInt32(); //
                        T.PriceByGoldForH = AV2.ReadInt32(); //
                        T.PriceByCashForH = AV2.ReadInt32(); //
                        T.HabilitarMes = AV2.ReadInt32(); //Mes
                        T.PriceByGoldForM = AV2.ReadInt32();
                        T.PriceByCashForM = AV2.ReadInt32();
                        T.HabilitarDia = AV2.ReadInt32(); //
                        T.PriceByGoldForD = AV2.ReadInt32(); //
                        T.PriceByCashForD = AV2.ReadInt32(); //
                        T.HabilitarIlimitado = AV2.ReadInt32(); //Etherno
                        T.PriceByGoldForI = AV2.ReadInt32();
                        T.PriceByCashForI = AV2.ReadInt32();
                        T.HabilitarGold = AV2.ReadByte(); //Activacion Gold
                        T.HabilitarCash = AV2.ReadByte(); //Activacion Cash

                        T.Unk12 = AV2.ReadByte();
                        T.Unk13 = AV2.ReadByte();

                        T.Delay = AV2.ReadInt32();
                        T.Pit_Angle = AV2.ReadInt32();
                        T.Attack = AV2.ReadInt32();
                        T.Defence = AV2.ReadInt32();
                        T.Energy = AV2.ReadInt32();
                        T.ItemSkipDelay = AV2.ReadInt32();
                        T.Shield_Recovery = AV2.ReadInt32();
                        T.Popularity = AV2.ReadInt32();
                       
                        T.Description = AV2.ReadPStringFixed(64);
                        T.RemainData = AV2.ReadBytes(448); ;//Aqui deve ser ajustado para o tamanho que esta sendo o bloco, nesse caso 644

                        Temp.Add(T); // Adiciona os dados lidos à lista


                        // Escreve os dados decriptados no arquivo temporário
                        fs.Write(AvatarDecrypted, 0, AvatarDecrypted.Length); // Escreve os dados decriptados
                        i++;
                    }

                    ShowData(Temp[0]);

                }
            }

        }

        //Função para abrir arquivos decriptados (também MH,MF,MB e etc)
        public void AbrirDec(string caminho)
        {
            if (caminho != null)
            {


                byte[] avatar = File.ReadAllBytes(caminho);

                // Leia os primeiros 4 bytes
                StreamDataReader AV2 = new StreamDataReader(avatar);
               int  numAvatar22 = AV2.ReadInt32();
                numAvatar = numAvatar22; // Isso lê os primeiros 4 bytes
                numAvatar2 = BitConverter.GetBytes(numAvatar);//Isso adiciona o mesmo valor acima, mas pra usar na encriptação depois, pode ser que isso mude, pois o CRC pode mudar se muda qualquer informação nos bytes.
                using (FileStream fs = new FileStream("Temp/AvatarDecriptado.dat", FileMode.Create, FileAccess.Write))
                {
                    // Escreve os 4 primeiros bytes de volta no início do arquivo
                    byte[] numAvatarBytes = BitConverter.GetBytes(numAvatar);
                    fs.Write(numAvatarBytes, 0, numAvatarBytes.Length); // Escreve os 4 bytes

                    int i = 0;
                    while (i < numAvatar)
                    {
                       
                        AvatarData T = new AvatarData();

                        // Leitura normal dos campos
                        T.ItemNo = AV2.ReadInt32();
                        T.ImgNo = AV2.ReadInt32();
                        T.IsNew = AV2.ReadInt32();
                        T.Name = AV2.ReadPStringFixed(23);
                        T.Visible = AV2.ReadByte(); //Visible
                        T.Unk03 = AV2.ReadByte();
                        T.HabilitarSemana = AV2.ReadByte(); //Semana
                        T.Unk04 = AV2.ReadByte();
                        T.Unk05 = AV2.ReadByte();
                        T.PriceByGoldForW = AV2.ReadInt32();
                        T.PriceByCashForW = AV2.ReadInt32();
                        T.Habilitar2Horas = AV2.ReadInt32(); //
                        T.PriceByGoldForH = AV2.ReadInt32(); //
                        T.PriceByCashForH = AV2.ReadInt32(); //
                        T.HabilitarMes = AV2.ReadInt32(); //Mes
                        T.PriceByGoldForM = AV2.ReadInt32();
                        T.PriceByCashForM = AV2.ReadInt32();
                        T.HabilitarDia = AV2.ReadInt32(); //
                        T.PriceByGoldForD = AV2.ReadInt32(); //
                        T.PriceByCashForD = AV2.ReadInt32(); //
                        T.HabilitarIlimitado = AV2.ReadInt32(); //Etherno
                        T.PriceByGoldForI = AV2.ReadInt32();
                        T.PriceByCashForI = AV2.ReadInt32();
                        T.HabilitarGold = AV2.ReadByte(); //Activacion Gold
                        T.HabilitarCash = AV2.ReadByte(); //Activacion Cash

                        T.Unk12 = AV2.ReadByte();
                        T.Unk13 = AV2.ReadByte();

                        T.Delay = AV2.ReadInt32();
                        T.Pit_Angle = AV2.ReadInt32();
                        T.Attack = AV2.ReadInt32();
                        T.Defence = AV2.ReadInt32();
                        T.Energy = AV2.ReadInt32();
                        T.ItemSkipDelay = AV2.ReadInt32();
                        T.Shield_Recovery = AV2.ReadInt32();
                        T.Popularity = AV2.ReadInt32();

                        T.Description = AV2.ReadPStringFixed(64);
                        if (i == 1347)
                        {
                            T.RemainData = AV2.ReadBytes(436);
                        }
                        else
                        {
                            T.RemainData = AV2.ReadBytes(448);
                        }

                        Temp.Add(T); // Adiciona os dados lidos à lista


                        // Escreve os dados decriptados no arquivo temporário
                       // fs.Write(AV2, 0, AV2.Length); // Escreve os dados decriptados
                        i++;
                    }

                    ShowData(Temp[0]);

                }
            }

        }

        ///


        public void SalvarWCDec()
        {
            SaveRow();

            // Usar um único MemoryStream e BinaryWriter para todo o processo
            using (MemoryStream ms = new MemoryStream())
            {

                using (BinaryWriter writer = new BinaryWriter(ms))
                {
                    StreamDataWrite Ouput = new StreamDataWrite();
                    Ouput.WriteBytes(numAvatar2);
                    MessageBox.Show(numAvatar.ToString());
                    for (int i = 0; i < numAvatar; i++)
                    {
                        writer.BaseStream.Position = 0;
                        // Limpar o MemoryStream antes de processar o próximo item
                        ms.SetLength(0);

                        // Escrever os dados no MemoryStream
                        writer.Write(Temp[i].ItemNo);
                        writer.Write(Temp[i].ImgNo);
                        writer.Write(Temp[i].IsNew);

                        // Processar o campo Name
                        if (Temp[i].Name.Length == 0)
                        {
                            writer.Write(new byte[23]);
                        }
                        else
                        {
                            // Escreve o nome em formato de array de bytes
                            writer.Write(Temp[i].Name.ToArray());

                            // Calcula quantos bytes faltam para completar 23
                            int padding = 23 - Temp[i].Name.Length;

                            // Preenche o restante com zeros, se necessário
                            if (padding > 0)
                            {
                                writer.Write(new byte[padding]);  // Escreve 'padding' zeros
                            }
                        }

                        writer.Write(Temp[i].Visible);
                        writer.Write(Temp[i].Unk03);
                        writer.Write(Temp[i].HabilitarSemana);
                        writer.Write(Temp[i].Unk04);
                        writer.Write(Temp[i].Unk05);
                        writer.Write(Temp[i].PriceByGoldForW);
                        writer.Write(Temp[i].PriceByCashForW);
                        writer.Write(Temp[i].Habilitar2Horas);
                        writer.Write(Temp[i].PriceByGoldForH);
                        writer.Write(Temp[i].PriceByCashForH);
                        writer.Write(Temp[i].HabilitarMes);
                        writer.Write(Temp[i].PriceByGoldForM);
                        writer.Write(Temp[i].PriceByCashForM);
                        writer.Write(Temp[i].HabilitarDia);
                        writer.Write(Temp[i].PriceByGoldForD);
                        writer.Write(Temp[i].PriceByCashForD);
                        writer.Write(Temp[i].HabilitarIlimitado);
                        writer.Write(Temp[i].PriceByGoldForI);
                        writer.Write(Temp[i].PriceByCashForI);
                        writer.Write(Temp[i].HabilitarGold);
                        writer.Write(Temp[i].HabilitarCash);

                        writer.Write(Temp[i].Unk12);
                        writer.Write(Temp[i].Unk13);

                        writer.Write(Temp[i].Delay);
                        writer.Write(Temp[i].Pit_Angle);
                        writer.Write(Temp[i].Attack);
                        writer.Write(Temp[i].Defence);
                        writer.Write(Temp[i].Energy);
                        writer.Write(Temp[i].ItemSkipDelay);
                        writer.Write(Temp[i].Shield_Recovery);
                        writer.Write(Temp[i].Popularity);

                        // Processar o campo Description
                        if (Temp[i].Description.Length == 0)
                        {
                            writer.Write(new byte[64]);
                        }
                        else
                        {
                            // Escreve o nome em formato de array de bytes
                            writer.Write(Temp[i].Description.ToArray());

                            // Calcula quantos bytes faltam para completar 23
                            int padding = 64 - Temp[i].Description.Length;

                            // Preenche o restante com zeros, se necessário
                            if (padding > 0)
                            {
                                writer.Write(new byte[padding]);  // Escreve 'padding' zeros
                            }
                        }

                        writer.Write(Temp[i].RemainData);

                        Ouput.WriteBytes(ms.ToArray());

                    }
                    File.WriteAllBytes("Temp/" + nomeArquivo, Ouput.ToByteArray());
                }
            }
        }
        public void EncriptarWC()
        {
            using (MemoryStream ms = new MemoryStream())
            {

                using (BinaryWriter writer = new BinaryWriter(ms))
                {
                    StreamDataWrite Ouput = new StreamDataWrite();
                    
                    Ouput.WriteBytes(numAvatar2);

                    for (int i = 0; i < numAvatar; i++)
                    {
                        SaveRow();
                        byte[] encryptedData = new byte[256];
                        writer.BaseStream.Position = 0;
                        ms.SetLength(0);
                        writer.Write(Temp[i].ItemNo);
                        writer.Write(Temp[i].ImgNo);
                        writer.Write(Temp[i].IsNew);

                        if (Temp[i].Name.Length == 0)
                        {
                            writer.Write(new byte[23]);
                        }
                        else
                        {
                            writer.Write(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(Temp[i].Name)); 

                            int padding = 23 - Temp[i].Name.Length;

                            if (padding > 0)
                            {
                                writer.Write(new byte[padding]); 
                            }
                        }

                        writer.Write(Temp[i].Visible);
                        writer.Write(Temp[i].Unk03);
                        writer.Write(Temp[i].HabilitarSemana);
                        writer.Write(Temp[i].Unk04);
                        writer.Write(Temp[i].Unk05);
                        writer.Write(Temp[i].PriceByGoldForW);
                        writer.Write(Temp[i].PriceByCashForW);
                        writer.Write(Temp[i].Habilitar2Horas);
                        writer.Write(Temp[i].PriceByGoldForH);
                        writer.Write(Temp[i].PriceByCashForH);
                        writer.Write(Temp[i].HabilitarMes);
                        writer.Write(Temp[i].PriceByGoldForM);
                        writer.Write(Temp[i].PriceByCashForM);
                        writer.Write(Temp[i].HabilitarDia);
                        writer.Write(Temp[i].PriceByGoldForD);
                        writer.Write(Temp[i].PriceByCashForD);
                        writer.Write(Temp[i].HabilitarIlimitado);
                        writer.Write(Temp[i].PriceByGoldForI);
                        writer.Write(Temp[i].PriceByCashForI);
                        writer.Write(Temp[i].HabilitarGold);
                        writer.Write(Temp[i].HabilitarCash);

                        writer.Write(Temp[i].Unk12);
                        writer.Write(Temp[i].Unk13);

                        writer.Write(Temp[i].Delay);
                        writer.Write(Temp[i].Pit_Angle);
                        writer.Write(Temp[i].Attack);
                        writer.Write(Temp[i].Defence);
                        writer.Write(Temp[i].Energy);
                        writer.Write(Temp[i].ItemSkipDelay);
                        writer.Write(Temp[i].Shield_Recovery);
                        writer.Write(Temp[i].Popularity);

                        if (Temp[i].Description.Length == 0)
                        {
                            writer.Write(new byte[64]);
                        }
                        else
                        {
                            writer.Write(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(Temp[i].Description));

                            int padding = 64 - Temp[i].Description.Length;

                            if (padding > 0)
                            {
                                writer.Write(new byte[padding]);
                            }
                        }

                        writer.Write(Temp[i].RemainData);

                        encryptedData = GBCrypto.Compress(ms.ToArray());


                        Ouput.WriteBytes(Temp[i].CRCInicial);
                        Ouput.WriteBytes(encryptedData);

                        Array.Clear(encryptedData, 0, encryptedData.Length);
                        
                    }
                    File.WriteAllBytes("Temp/" + nomeArquivo, Ouput.ToByteArray());
                }
            }
        }



        

        void SaveRow()
        {
            if (!string.IsNullOrEmpty(nomeArquivo))
            {


                // Temp[CurrentIndex].MapName = TXTTITLE.Text;
                Temp[CurrentIndex].Name = txtNome.Text;
                Temp[CurrentIndex].Description = txtDesc.Text;
                //Temp[CurrentIndex].imgNum = txtImgNum.Text;
                Temp[CurrentIndex].PriceByCashForW = Int32.Parse(txtCashW.Text);
                Temp[CurrentIndex].PriceByCashForH = Int32.Parse(txtCash2H.Text);
                Temp[CurrentIndex].PriceByCashForD = Int32.Parse(txtCash1D.Text);
                Temp[CurrentIndex].PriceByCashForM = Int32.Parse(txtCashM.Text);
                Temp[CurrentIndex].PriceByCashForI = Int32.Parse(txtCashi.Text);
                Temp[CurrentIndex].PriceByGoldForH = Int32.Parse(txtGold2H.Text);
                Temp[CurrentIndex].PriceByGoldForD = Int32.Parse(txtGold1Dia.Text);
                Temp[CurrentIndex].PriceByGoldForW = Int32.Parse(txtGoldW.Text);
                Temp[CurrentIndex].PriceByGoldForM = Int32.Parse(txtGoldM.Text);
                Temp[CurrentIndex].PriceByGoldForI = Int32.Parse(txtGoldI.Text);
                Temp[CurrentIndex].Delay = Int32.Parse(txtDelay.Text);
                Temp[CurrentIndex].Defence = Int32.Parse(txtDefense.Text);
                Temp[CurrentIndex].Energy = Int32.Parse(txtEnergy.Text);
                Temp[CurrentIndex].Pit_Angle = Int32.Parse(txtPit.Text);
                Temp[CurrentIndex].Popularity = Int32.Parse(txtPopula.Text);
                Temp[CurrentIndex].Shield_Recovery = Int32.Parse(txtShielRec.Text);
                Temp[CurrentIndex].ItemSkipDelay = Int32.Parse(txtSkipDelay.Text);
                Temp[CurrentIndex].Attack = Int32.Parse(txtAtack.Text);
                Temp[CurrentIndex].ImgNo = Int32.Parse(txtImgNum.Text);
                Temp[CurrentIndex].codigoAv = Int32.Parse(txtCodigo.Text);
                Temp[CurrentIndex].IsNew = Int32.Parse(txtIsNew.Text);
                if ((bool)chkCash.Checked == true)
                {
                    Temp[CurrentIndex].HabilitarCash = 0x1;
                }
                else
                {
                    Temp[CurrentIndex].HabilitarCash = 0x0;
                }
                if ((bool)chkGold.Checked == true)
                {
                    Temp[CurrentIndex].HabilitarGold = 0x1;
                }
                else
                {
                    Temp[CurrentIndex].HabilitarGold = 0x0;
                }
                if ((bool)chkVisivel.Checked == true)
                {
                    Temp[CurrentIndex].Visible = 0x1;
                }
                else
                {
                    Temp[CurrentIndex].Visible = 0x0;
                }

            }
        }

        public string BytesToHexString(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", "");
        }

        public byte[] HexStringToBytes(string hex)
        {
            int length = hex.Length;
            byte[] bytes = new byte[length / 2];
            for (int i = 0; i < length; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }

        void ShowData(AvatarData Data)
        {

            chkCash.Checked = false;
            chkGold.Checked = false;
            chkVisivel.Checked = false;
            txtNome.Text = "";
            txtDesc.Text = "";
            txtCashi.Text = "";
            txtCashM.Text = "";
            txtCashW.Text = "";
            txtGoldI.Text = "";
            txtGoldM.Text = "";
            txtGoldW.Text = "";
            txtDelay.Text = "";
            txtDefense.Text = "";
            txtEnergy.Text = "";
            txtPit.Text = "";
            txtPopula.Text = "";
            txtShielRec.Text = "";
            txtSkipDelay.Text = "";
            txtAtack.Text = "";
            /////////////////////////////////////
            lblMax.Visible = true;
            LBLINDEX.Visible = true;
            LBLINDEX.Text = "Avatar Atual : " + CurrentIndex;
            lblMax.Text = "Num. de Avatares : " + numAvatar.ToString();
            label19.Text = "Arquivo atual : " + nomeArquivo;
            txtNome.Text = Data.Name;
            txtDesc.Text = Data.Description;
            txtCashi.Text = Data.PriceByCashForI.ToString();
            txtCashM.Text = Data.PriceByCashForM.ToString();
            txtCashW.Text = Data.PriceByCashForW.ToString();
            txtGoldI.Text = Data.PriceByGoldForI.ToString();
            txtGoldM.Text = Data.PriceByGoldForM.ToString();
            txtGoldW.Text = Data.PriceByGoldForW.ToString();
            txtDelay.Text = Data.Delay.ToString();
            txtDefense.Text = Data.Defence.ToString();
            txtEnergy.Text = Data.Energy.ToString();
            txtPit.Text = Data.Pit_Angle.ToString();
            txtPopula.Text = Data.Popularity.ToString();
            txtShielRec.Text = Data.Shield_Recovery.ToString();
            txtSkipDelay.Text = Data.ItemSkipDelay.ToString();
            txtAtack.Text = Data.Attack.ToString();
            txtImgNum.Text = Data.ImgNo.ToString();
            txtCodigo.Text = Data.ItemNo.ToString();
            txtCash2H.Text = Data.PriceByCashForH.ToString();
            txtCash1D.Text = Data.PriceByCashForD.ToString();
            txtGold1Dia.Text = Data.PriceByGoldForD.ToString();
            txtGold2H.Text = Data.PriceByGoldForH.ToString();
            txtIsNew.Text = Data.IsNew.ToString();
            //txtImgNum.Text = Data.imgNum.ToString();
            /*txtCodigo.Text = Data.codigoAv.ToString();
            txtRemain1.Text = BytesToHexString(Data.RemainData1);
            txtRemain2.Text = BytesToHexString(Data.RemainData2);
            txtRemain3.Text = BytesToHexString(Data.RemainData3);
            txtRemain4.Text = BytesToHexString(Data.RemainData4);*/
            if (Data.HabilitarCash == 1)
            {
                chkCash.Checked = true;

            }
            if (Data.HabilitarGold == 1)
            {
                chkGold.Checked = true;
            }

            if (Data.Visible == 1)
            {
                chkVisivel.Checked = true;
            }
            if (Data.Habilitar2Horas == 1)
            {
                chk2H.Checked = true;
            }
            if (Data.HabilitarDia == 1)
            {
                chk1Dia.Checked = true;
            }
            if (Data.HabilitarMes == 1)
            {
                chk1Mes.Checked = true;
            }
            if (Data.HabilitarIlimitado == 1)
            {
                chkEterno.Checked = true;
            }
        }

        private void btnProx_Click(object sender, EventArgs e)
        {
            SaveRow();
            if (CurrentIndex + 1 < Temp.Count)
            {
                CurrentIndex++;
                ShowData(Temp[CurrentIndex]);
            }
        }


        
        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void btnAnt_Click(object sender, EventArgs e)
        {
            SaveRow();
            if (CurrentIndex > -1)
            {
                if (CurrentIndex - 1 > -1)
                {
                    CurrentIndex--;
                    ShowData(Temp[CurrentIndex]);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveRow();
            EncriptarWC();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtIr.Text))
            {
                int number = Convert.ToInt32(txtIr.Text);
                SaveRow();
                CurrentIndex = number;
                ShowData(Temp[CurrentIndex]);
            }


        }


        private void avatardatToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void abrirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lblMax.Visible = true;
            LBLINDEX.Visible = true;
            label19.Visible = true;
            //Descomente aqui se quiser que abra o programa já com o arquivo decriptado. 
           // Decriptar(OpenFile());
        }

        private string OpenFile()
        {
            // Garante que qualquer FileStream existente seja fechado e liberado corretamente
            if (FileStream != null)
            {
                FileStream.Close();
                FileStream.Dispose();
                FileStream = null;
            }

            // Cria uma nova instância do OpenFileDialog
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // Define o diretório inicial como a pasta onde o executável está
            openFileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // Define as propriedades do OpenFileDialog (opcional)
            openFileDialog.Filter = "Dat Files (*.Dat)|*.Dat";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true; 

            // Exibe a caixa de diálogo e verifica se o usuário selecionou um arquivo
            DialogResult result = openFileDialog.ShowDialog();

            // Verifica se o usuário clicou em OK, ou seja, selecionou um arquivo
            if (result == DialogResult.OK)
            {
                txtAtack.Text = "";
                txtCash1D.Text = "";
                txtCash2H.Text = "";
                txtCashi.Text = "";
                txtCashM.Text = "";
                txtCashW.Text = "";
                txtCodigo.Text = "";
                txtDefense.Text = "";
                txtDelay.Text = "";
                txtDesc.Text = "";
                txtEnergy.Text = "";
                txtGcoin1D.Text = "";
                txtGCoin2H.Text = "";
                txtGI.Text = "";
                txtGM.Text = "";
                txtGold1Dia.Text = "";
                txtGold2H.Text = "";
                txtGoldI.Text = "";
                txtGoldM.Text = "";
                txtGoldW.Text = "";
                txtGW.Text = "";

                txtImgNum.Text = "";
                txtIr.Text = "";
                txtIsNew.Text = "";
                txtNome.Text = "";
                txtPit.Text = "";
                txtPopula.Text = "";
                txtShielRec.Text = "";
                txtSkipDelay.Text = "";

                // Verifica se o arquivo existe e é válido
                if (!string.IsNullOrEmpty(openFileDialog.FileName) && File.Exists(openFileDialog.FileName))
                {
                    string fullPath = openFileDialog.FileName;

                    // Obtém apenas o nome do arquivo (sem o caminho completo)
                    string fileName = Path.GetFileName(fullPath);

                    // Exibe o nome do arquivo
                    nomeArquivo = fileName;
                    // Retorna o caminho completo do arquivo selecionado
                    return openFileDialog.FileName;

                }
            }

            // Se o usuário cancelar ou não selecionar nenhum arquivo, retorna null ou uma string vazia
            return null;
        }

        private void salvarEncriptadoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(nomeArquivo))
            {
                EncriptarWC();
            }

        }

        private void abrirEncriptadoToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            lblMax.Visible = true;
            LBLINDEX.Visible = true;
            label19.Visible = true;
            DecriptarWC(OpenFile());
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void salvarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(nomeArquivo))
            {
                SalvarWCDec();
            }

        }

        private void abrirToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            lblMax.Visible = true;
            LBLINDEX.Visible = true;
            label19.Visible = true;
            AbrirDec(OpenFile());
        }
    }

}
