using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using FontAwesome.WPF;
using Microsoft.Win32;

namespace Lab5
{
    internal class MainVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }

        private int downloadPercentComplete;
        public int DownloadPercentComplete
        {
            get { return downloadPercentComplete; }
            set
            {
                downloadPercentComplete = value;
                DownloadPercentCompleteString = $"Downloading {DownloadingFile} ... {downloadPercentComplete}% Complete";
                NotifyPropertyChanged();
            }
        }

        private string downloadingFile;

        public string DownloadingFile
        {
            get { return downloadingFile; }
            set { downloadingFile = value; NotifyPropertyChanged(); }
        }

        private int percentComplete2;
        public int ValidationStepsPercentageComplete
        {
            get { return percentComplete2; }
            set
            {
                percentComplete2 = value;
                NotifyPropertyChanged();
            }
        }

        private string downloadPercentCompleteString;

        public string DownloadPercentCompleteString
        {
            get { return downloadPercentCompleteString; }
            set { downloadPercentCompleteString = value; NotifyPropertyChanged(); }
        }

        private int validationStepsCompleted;

        public int ValidationStepsCompleted
        {
            get { return validationStepsCompleted; }
            set { 
                validationStepsCompleted = value;
                ValidationStepsPercentageComplete = value * 10;
                NotifyPropertyChanged();
                ValidationStepsCompletedString = $"Validating... Step {ValidationStepsCompleted} of 10";
            }
        }


        private string validationStepsCompletedString;

        public string ValidationStepsCompletedString
        {
            get { return validationStepsCompletedString; }
            set { validationStepsCompletedString = value; NotifyPropertyChanged(); }
        }

        private string isoUrl = "https://cdimage.debian.org/cdimage/archive/9.13.0/amd64/iso-cd/debian-9.13.0-amd64-xfce-CD-1.iso";

        public string IsoUrl
        {
            get { return isoUrl; }
            set { isoUrl = value; NotifyPropertyChanged(); }
        }
        private string isoNameStatus;

        public string IsoNameStatus
        {
            get { return isoNameStatus; }
            set
            {
                isoNameStatus = value;
                NotifyPropertyChanged();
                if (value == "Check")
                    CalcChecksumsEnabled = true;
                else
                    CalcChecksumsEnabled = false;
            }
        }

        private bool isoNameStatusSpin;
        public bool IsoNameStatusSpin
        {
            get { return isoNameStatusSpin; }
            set { isoNameStatusSpin = value; NotifyPropertyChanged(); }
        }

        private string md5status;
        public string Md5Status
        {
            get { return md5status; }
            set { md5status = value; NotifyPropertyChanged(); }
        }

        private bool md5statusSpin;
        public bool Md5statusSpin
        {
            get { return md5statusSpin; }
            set { md5statusSpin = value; NotifyPropertyChanged(); }
        }

        private string sha1status;
        public string Sha1Status
        {
            get { return sha1status; }
            set { sha1status = value; NotifyPropertyChanged(); }
        }

        private bool sha1statusSpin;
        public bool Sha1statusSpin
        {
            get { return sha1statusSpin; }
            set { sha1statusSpin = value; NotifyPropertyChanged(); }
        }

        private string sha256status;
        public string Sha256Status
        {
            get { return sha256status; }
            set { sha256status = value; NotifyPropertyChanged(); }
        }

        private bool sha256statusSpin;
        public bool Sha256statusSpin
        {
            get { return sha256statusSpin; }
            set { sha256statusSpin = value; NotifyPropertyChanged(); }
        }

        private string sha512status;
        public string Sha512Status
        {
            get { return sha512status; }
            set { sha512status = value; NotifyPropertyChanged(); }
        }

        private bool sha512statusSpin;
        public bool Sha512statusSpin
        {
            get { return sha512statusSpin; }
            set { sha512statusSpin = value; NotifyPropertyChanged(); }
        }

        private string isoName;

        public string IsoName
        {
            get { return isoName; }
            set
            {
                isoName = value;
                NotifyPropertyChanged();
            }
        }
        public ICommand DownloadCommand { get; set; }
        public ICommand CalculateChecksumsCommand { get; set; }
        public ICommand OpenFileCommand { get; set; }

        private bool calcChecksumsEnabled;

        public bool CalcChecksumsEnabled
        {
            get { return calcChecksumsEnabled; }
            set { calcChecksumsEnabled = value; NotifyPropertyChanged(); }
        }


        private string md5CheckSum;
        public string Md5CheckSum
        {
            get { return md5CheckSum; }
            set { md5CheckSum = value; NotifyPropertyChanged(); }
        }

        private string sha1CheckSum;
        public string Sha1CheckSum
        {
            get { return sha1CheckSum; }
            set { sha1CheckSum = value; NotifyPropertyChanged(); }
        }

        private string sha256CheckSum;
        public string Sha256CheckSum
        {
            get { return sha256CheckSum; }
            set { sha256CheckSum = value; NotifyPropertyChanged(); }
        }

        private string sha512CheckSum;
        public string Sha512CheckSum
        {
            get { return sha512CheckSum; }
            set { sha512CheckSum = value; NotifyPropertyChanged(); }
        }

        public string[] Md5Sums { get; set; }
        public string Md5providedChecksum { get; set; }
        public string Sha1providedChecksum { get; set; }
        public string Sha256providedChecksum { get; set; }
        public string Sha512providedChecksum { get; set; }
        public string[] Sha1Sums { get; set; }
        public string[] Sha256Sums { get; set; }
        public string[] Sha512Sums { get; set; }
        public string ISOPATH { get; set; }
        public delegate void EventHandler(object sender, EventArgs e);

        public MainVM()
        {
            DownloadCommand = new RelayCommand(Download);
            CalculateChecksumsCommand = new RelayCommand(CalculateChecksums);
            OpenFileCommand = new RelayCommand(OpenFile);
        }
        private async void Download()
        {
            WebClient client = new WebClient();

            IsoNameStatus = "Refresh";
            IsoNameStatusSpin = true;
            IsoName = IsoUrl.Substring(IsoUrl.LastIndexOf('/') + 1);
            client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(UpdatePercentages);
            await client.DownloadFileTaskAsync(IsoUrl, IsoName);
            IsoNameStatusSpin = false;
            IsoNameStatus = "Check";

        }
        public void OpenFile()
        {
            OpenFileDialog ofd = new OpenFileDialog();

            if ((bool)ofd.ShowDialog())
                ISOPATH = ofd.FileName;
            DownloadPercentComplete = 100;
            ValidationStepsCompleted = 0;
            IsoNameStatus = "Check";
            IsoName = ofd.FileName.Substring(ofd.FileName.LastIndexOf('\\') + 1);
        }
        public void UpdatePercentages(object sender, DownloadProgressChangedEventArgs e)
        {
            DownloadPercentComplete = e.ProgressPercentage;
        }
        private async void CalculateChecksums()
        {
            #region Setup and Get Downloads
            ValidationStepsCompleted = 0;
            Stream streamMd5 = File.OpenRead(IsoName);
            Stream streamSha1 = File.OpenRead(IsoName);
            Stream streamSha256 = File.OpenRead(IsoName);
            Stream streamSha512 = File.OpenRead(IsoName);
            MD5 md5 = MD5.Create();
            SHA1 sha1 = SHA1.Create();
            SHA256 sha256 = SHA256.Create();
            SHA512 sha512 = SHA512.Create();
            string FolderUrl = IsoUrl.Substring(0, IsoUrl.LastIndexOf('/') + 1);
            string isoFolder = "";
            if (!string.IsNullOrEmpty(ISOPATH))
                isoFolder = ISOPATH.Substring(0, ISOPATH.LastIndexOf('\\'));
            WebClient client = new WebClient();
            client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(UpdatePercentages);
            string md5url = FolderUrl + "MD5SUMS";
            string sha1url = FolderUrl + "SHA1SUMS";
            string sha256url = FolderUrl + "SHA256SUMS";
            string sha512url = FolderUrl + "SHA512SUMS";
            if (!File.Exists(isoFolder + "MD5SUMS"))
            {
                DownloadingFile = "MD5SUMS";
                DownloadPercentComplete = 0;
                await client.DownloadFileTaskAsync(md5url, "MD5SUMS");

            }
            ValidationStepsCompleted++;
            if (!File.Exists(isoFolder + "SHA1SUMS"))
            {
                DownloadingFile = "SHA1SUMS";
                DownloadPercentComplete = 0;
                await client.DownloadFileTaskAsync(sha1url, "SHA1SUMS"); 
            }
            ValidationStepsCompleted++;
            if (!File.Exists(isoFolder + "SHA256SUMS"))
            {
                DownloadingFile = "SHA256";
                DownloadPercentComplete = 0;
                await client.DownloadFileTaskAsync(sha256url, "SHA256SUMS");
            }
            ValidationStepsCompleted++;
            if (!File.Exists(isoFolder + "SHA512SUMS"))
            {
                DownloadingFile = "SHA512SUMS";
                DownloadPercentComplete = 0;
                await client.DownloadFileTaskAsync(sha512url, "SHA512SUMS"); 
            }
            ValidationStepsCompleted++;
            #endregion

            #region MD5 STUFF

            Md5Status = "Refresh";
            Md5statusSpin = true;
            //compute our own MD5 checksum
            Md5CheckSum = await ComputeChecksum(md5, streamMd5);
            //get MD5 checksums from source to verify match
            Md5statusSpin = false;
            Md5Status = "Check";
            // Note: probably would've been better to use dictionaries for matching iso filenames to checksums
            string[] md5Contents = File.ReadAllLines("MD5SUMS");
            string[] md5providedSums = new string[md5Contents.Length * 2];
            for (int i = 0; i < md5Contents.Length; i++)
            {
                md5providedSums[i * 2] = md5Contents[i].Substring(0, 32);
                md5providedSums[i * 2 + 1] = md5Contents[i].Substring(34);
            }
            Md5Sums = md5providedSums;
            for (int i = 0; i < md5providedSums.Length - 1; i += 2)
                if (md5providedSums[i + 1] == IsoName)
                    Md5providedChecksum = Md5Sums[i];

            if (Md5CheckSum == Md5providedChecksum)
            {
                Md5Status = "CheckCircle";
                ValidationStepsCompleted++;
            }
            #endregion

            #region SHA1 STUFF

            Sha1Status = "Refresh";
            Sha1statusSpin = true;
            Sha1CheckSum = await ComputeChecksum(sha1, streamSha1);
            //get SHA1 checksums from source
            //await client.DownloadFileTaskAsync(FolderUrl + "SHA1SUMS", "SHA1SUMS");
            Sha1statusSpin = false;
            Sha1Status = "Check";
            string[] sha1Contents = File.ReadAllLines("SHA1SUMS");
            string[] sha1providedSums = new string[sha1Contents.Length * 2];
            for (int i = 0; i < sha1Contents.Length; i++)
            {
                sha1providedSums[i * 2] = sha1Contents[i].Substring(0, 40);
                sha1providedSums[i * 2 + 1] = sha1Contents[i].Substring(42);
            }
            Sha1Sums = sha1providedSums;
            for (int i = 0; i < Sha1Sums.Length - 1; i += 2)
                if (Sha1Sums[i + 1] == IsoName)
                    Sha1providedChecksum = Sha1Sums[i];

            if (Sha1CheckSum == Sha1providedChecksum)
            {
                Sha1Status = "CheckCircle";
                ValidationStepsCompleted++;
            }
            #endregion

            #region SHA256 STUFF

            //get SHA256 checksums from source
            Sha256Status = "Refresh";
            Sha256statusSpin = true;
            Sha256CheckSum = await ComputeChecksum(sha256, streamSha256);
            Sha256statusSpin = false;
            Sha256Status = "Check";
            string[] sha256Contents = File.ReadAllLines("SHA256SUMS");
            string[] sha256providedSums = new string[sha256Contents.Length * 2];
            for (int i = 0; i < sha256Contents.Length; i++)
            {
                sha256providedSums[i * 2] = sha256Contents[i].Substring(0, 64);
                sha256providedSums[i * 2 + 1] = sha256Contents[i].Substring(66);
            }
            Sha256Sums = sha256providedSums;
            for (int i = 0; i < Sha256Sums.Length - 1; i += 2)
                if (Sha256Sums[i + 1] == IsoName)
                    Sha256providedChecksum = Sha256Sums[i];

            if (Sha256CheckSum == Sha256providedChecksum)
            {
                Sha256Status = "CheckCircle";
                ValidationStepsCompleted++;
            }
            #endregion

            #region SHA512 STUFF

            //get SHA512 checksums from source
            Sha512Status = "Refresh";
            Sha512statusSpin = true;
            Sha512CheckSum = await ComputeChecksum(sha512, streamSha512);
            Sha512statusSpin = false;
            Sha512Status = "Check";
            string[] sha512Contents = File.ReadAllLines("SHA512SUMS");
            string[] sha512providedSums = new string[sha512Contents.Length * 2];
            for (int i = 0; i < sha512Contents.Length; i++)
            {
                sha512providedSums[i * 2] = sha512Contents[i].Substring(0, 128);
                sha512providedSums[i * 2 + 1] = sha512Contents[i].Substring(130);
            }
            Sha512Sums = sha512providedSums;
            for (int i = 0; i < Sha512Sums.Length - 1; i += 2)
                if (Sha512Sums[i + 1] == IsoName)
                    Sha512providedChecksum = Sha512Sums[i];

            if (Sha512CheckSum == Sha512providedChecksum) 
            {
                Sha512Status = "CheckCircle";
                ValidationStepsCompleted++;
            }
            #endregion
        }
        private async Task<string> ComputeChecksum(HashAlgorithm halg, Stream data)
        {
            byte[] hashedData = await halg.ComputeHashAsync(data);
            return BitConverter.ToString(hashedData).Replace("-", "").ToLower();
        }
    }
}
