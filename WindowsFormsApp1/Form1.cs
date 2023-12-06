using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        String KeyASE;
        String IVASE;

        public Form1()
        {
            InitializeComponent();
        }


        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String tarjetaCuenta = textBox1.Text + textBox2.Text + textBox3.Text + textBox4.Text;

            // sanitizar cadena. válida que la cadena sean números y 16 carácteres.

            if (Regex.IsMatch(tarjetaCuenta, "^[0-9]{16}$"))
            {
                textBox5.Text = tarjetaCuenta; 
                
                // muestra la siguiente opción
                groupBox2.Visible = true;

            }
            else {
                MessageBox.Show("Agregar cuatro números en cada uno de los campos.\nPor ejemplo : 5689 5958 8598 8594");
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            String cadenaPrincipal = textBox5.Text;

            string hash = CalcularSHA256(cadenaPrincipal);

            textBox6.Text = hash;


        }

        static string CalcularSHA256(string s)
        {
            StringBuilder obtieneCadena = new StringBuilder();

            string hash = String.Empty;

            // inicia
            using (SHA256 sha256 = SHA256.Create())
            {
                // Calcula hash
                byte[] hashValue = sha256.ComputeHash(Encoding.UTF8.GetBytes(s));

                // convierte en cadea
                foreach (byte b in hashValue)
                {


                    obtieneCadena.Append($"{b:X2}");
                }
            }

            hash = obtieneCadena.ToString();

            return hash;
        }

        private void button3_Click(object sender, EventArgs e)
        {

            // Crea key
            KeyASE = CreateKey();
            Console.WriteLine("KeyASE: " + KeyASE);

            // Crea IV
            IVASE = CreateIV();

            string plaintext = textBox5.Text;
            string encryptedText;

            // Encrypt
            encryptedText = EncryptDataWithAes(plaintext, KeyASE, IVASE);
            textBox7.Text = encryptedText;

        }

        private static string EncryptDataWithAes(string plainText, string keyBase64, string IVBase64)
        {
            using (Aes aesAlgorithm = Aes.Create())
            {
                aesAlgorithm.Key = Convert.FromBase64String(keyBase64);
                aesAlgorithm.IV = Convert.FromBase64String(IVBase64) ;

                // Crea objeto
                ICryptoTransform encryptor = aesAlgorithm.CreateEncryptor();

                byte[] encryptedData;

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(plainText);
                        }
                        encryptedData = ms.ToArray();
                    }
                }

                return Convert.ToBase64String(encryptedData);
            }
        }


        private static string DecryptDataWithAes(string cipherText, string keyBase64, string IVBase64)
        {
            using (Aes aesAlgorithm = Aes.Create())
            {
                aesAlgorithm.Key = Convert.FromBase64String(keyBase64);
                aesAlgorithm.IV = Convert.FromBase64String(IVBase64);

                // Crea objeto
                ICryptoTransform decryptor = aesAlgorithm.CreateDecryptor();

                byte[] cipher = Convert.FromBase64String(cipherText);

                using (MemoryStream ms = new MemoryStream(cipher))
                {
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(cs))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
        }


        private static string CreateKey()
        {

            using (Aes aesAlgorithm = Aes.Create())
            {
                aesAlgorithm.KeySize = 256;
                aesAlgorithm.GenerateKey();
                string keyBase64 = Convert.ToBase64String(aesAlgorithm.Key);

                return keyBase64;

            }
       
        }


        private static string CreateIV()
        {

            using (Aes aesAlgorithm = Aes.Create())
            {
                aesAlgorithm.KeySize = 256;
                aesAlgorithm.GenerateIV();
                string IVBase64 = Convert.ToBase64String(aesAlgorithm.IV);

                return IVBase64;


            }

        }

        private void button4_Click(object sender, EventArgs e)
        {


            string decryptText;

               if (string.IsNullOrEmpty(textBox7.Text)) {

                  MessageBox.Show("Debe de cifrar el número de la tarjeta");

               }
               else { 

                    decryptText = DecryptDataWithAes(textBox7.Text, KeyASE, IVASE);
                    textBox8.Text = decryptText;
                }



        }

        private void button5_Click(object sender, EventArgs e)
        {
            String cadenaPrincipal = textBox8.Text;


            if (string.IsNullOrEmpty(textBox7.Text))
            {

                MessageBox.Show("Debe de descifrar el número de la tarjeta");

            }
            else
            {

                string hash = CalcularSHA256(cadenaPrincipal);

                textBox9.Text = hash;

            }

        }
    }

}
