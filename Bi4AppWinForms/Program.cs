using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

// Bi4App - Projeto de Exemplo de implementação (Desktop em C#)
// Autor: Simão Brandão
// Email: sibrandao2008@gmail.com 
// Telemovel: +244 948 493 828 
// Linkedin: https://linkedin.com/in/SimaoBrandao 
// Github: https://github.com/SimaoBrandao/Bi4AppWinForms.git
// Nuget:  https://www.nuget.org/packages/Bi4App
// Data: 09/01/2025 

namespace Bi4AppWinForms
{
    static class Program
    {
        /// <summary>
        /// Ponto de entrada principal para o aplicativo.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
