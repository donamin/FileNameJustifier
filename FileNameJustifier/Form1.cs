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
using System.Threading;

namespace FileNameJustifier
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                txtFolderAddress.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void btnRename_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(RenameAllFiles);
            thread.Start();
        }

        void RenameAllFiles()
        {
            string[] listFolders = Directory.GetDirectories(txtFolderAddress.Text);
            Log("Going through folders...");
            for (int i = 0; i < listFolders.Length; i++)
            {
                string folderPath = listFolders[i];
                string folderName = folderPath.Substring(folderPath.LastIndexOf('\\') + 1);
                Log("Folder " + folderName + " (#" + (i + 1) + " of " + listFolders.Length + ")...");
                if (folderName.Length != 3)
                {
                    Log("Folder " + folderName + " skipped.");
                }
                else
                {
                    string[] listFiles = Directory.GetFiles(folderPath);
                    for (int f = 0; f < listFiles.Length; f++)
                    {
                        try
                        {
                            string filePath = listFiles[f];
                            string fileName = filePath.Substring(filePath.LastIndexOf('\\') + 1);

                            char[] fileNameArray = fileName.ToCharArray();
                            int indexUnderline = fileName.IndexOf('_', fileName.IndexOf('_') + 1);
                            fileNameArray[indexUnderline + 1] = folderName[0];
                            fileNameArray[indexUnderline + 2] = folderName[1];

                            indexUnderline = fileName.LastIndexOf('_', fileName.LastIndexOf('_') - 1);
                            fileNameArray[indexUnderline + 1] = folderName[2];

                            string newFileName = new string(fileNameArray);
                            string newFilePath = filePath.Substring(0, filePath.LastIndexOf('\\') + 1) + newFileName;
                            File.Move(filePath, newFilePath);
                        }
                        catch(Exception ex)
                        {
                            Log("Exception: " + ex.Message);
                        }
                    }
                    Log(listFiles.Length + " files renamed successfully at folder " + folderName);
                }
                Log("---------------------------------------------------");
            }
            Log("Done :)");
        }

        void Log(string message)
        {
            lstLog.Items.Add(message);
        }
    }
}