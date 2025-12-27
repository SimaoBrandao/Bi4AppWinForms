using AForge.Video;
using AForge.Video.DirectShow;
using BiApp;
using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ZXing;

// Bi4App - Projeto de Demonstração (Desktop em C#)
// Autor: Simão Brandão
// Email: sibrandao2008@gmail.com 
// Telemovel: +244 948493828 
// Github: https://github.com/SimaoBrandao/Bi4AppWinForms.git
// Nuget:  https://www.nuget.org/packages/Bi4App
// Data: 09/01/2025 

namespace Bi4AppWinForms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            // Ativa captura silenciosa do POS Scanner (keyboard wedge)
            this.KeyPreview = true;
        }

        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;
        private bool leituraEmAndamento = false;
        private bool qrDetectado = false;

        //   CAMPOS POS SCANNER
        private readonly StringBuilder posBuffer = new StringBuilder();
        private DateTime ultimoInput = DateTime.Now;
        private readonly TimeSpan timeoutPOS = TimeSpan.FromMilliseconds(300);
        private void IniciarLeitura()
        {
            qrDetectado = false;
            PararCamera();

            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (videoDevices.Count == 0)
            {
                MessageBox.Show("Nenhuma câmera encontrada.");
                return;
            }

            videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
            videoSource.NewFrame += video_NewFrame;
            videoSource.Start();

            leituraEmAndamento = true;
            BntQRCode.Text = "Parar Leitura";
        }

        private void PararLeituraManual()
        {
            PararCamera();

            leituraEmAndamento = false;
            BntQRCode.Text = "Iniciar Leitura";

            pbVideo.Image?.Dispose();
            pbVideo.Image = null;
        }

        // ==========================
        //   FRAME DA CÂMERA
        // ==========================
        private void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            if (qrDetectado) return;

            try
            {
                using (var bitmap = (Bitmap)eventArgs.Frame.Clone())
                {
                    Invoke((MethodInvoker)(() =>
                    {
                        pbVideo.Image?.Dispose();
                        pbVideo.Image = (Bitmap)bitmap.Clone();
                    }));

                    var reader = new BarcodeReader();
                    var result = reader.Decode(bitmap);

                    if (result != null)
                    {
                        qrDetectado = true;

                        BeginInvoke((MethodInvoker)(() =>
                        {
                            PararCamera();

                            leituraEmAndamento = false;
                            BntQRCode.Text = "Iniciar Leitura";

                            pbVideo.Image = null;
                            Processar(result.Text);
                        }));
                    }
                }
            }
            catch
            {
                // erros ocasionais de frame ignorados
            }
        }
        private void Processar(string texto)
        {

            Bi4App bi4App = new Bi4App();
            var retorno = bi4App.ObterDados(texto);

            txtNomeCompleto.Text = retorno.NomeCompleto;
            txtNumeroBI.Text = retorno.NumeroBilhete;
            txtProvincia.Text = retorno.Provincia;
            txtDataNascimento.Text = retorno.DataNascimento;
            txtSexo.Text = retorno.Sexo;
            txtEstadoCivil.Text = retorno.EstadoCivil;
            txtDataEmissao.Text = retorno.DataEmissao;
            txtDataValidade.Text = retorno.DataValidade;
            txtProvinciaEmissora.Text = retorno.ProvinciaEmissora;
            txtIdade.Text = retorno.Idade.ToString();
            txtPrimeiroNome.Text = retorno.PrimeiroNome;
            txtUltimoNome.Text = retorno.UltimoNome;
            txtFaixaEtaria.Text = retorno.FaixaEtaria;
            txtEstadoValidade.Text = retorno.EstadoValidade;
            txtVersao.Text = retorno.Versao;
        }
        private void PararCamera()
        {
            if (videoSource != null)
            {
                if (videoSource.IsRunning)
                {
                    videoSource.NewFrame -= video_NewFrame;
                    videoSource.SignalToStop();
                    videoSource.WaitForStop();
                }

                videoSource = null;
            }
        }
        private void BntQRCode_Click(object sender, EventArgs e)
        {
            if (!leituraEmAndamento)
                IniciarLeitura();
            else
                PararLeituraManual();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            var agora = DateTime.Now;

            // Reinicia buffer se houve pausa longa
            if ((agora - ultimoInput) > timeoutPOS)
                posBuffer.Clear();

            ultimoInput = agora;

            // Enter finaliza leitura do código
            if (e.KeyChar == (char)Keys.Return || e.KeyChar == (char)Keys.LineFeed)
            {
                var texto = posBuffer.ToString().Trim();
                posBuffer.Clear();

                if (!string.IsNullOrWhiteSpace(texto))
                    Processar(texto);

                return;
            }

            // Acumula caracteres do scanner
            posBuffer.Append(e.KeyChar);
        }
    }
}
