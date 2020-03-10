using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Security.Cryptography;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Xml.Serialization;
using System.Collections.ObjectModel;
using rysio.Structures;

namespace rysio
{
	/// <summary>
	/// MainWindow logic
	/// </summary>
	public partial class MainWindow : Window
	{
		#region Fields

		private static readonly byte[] salt = Encoding.UTF8.GetBytes("mamy tracycję");

		public static readonly string programDataPath = "programData.xml";

		private ProgramData Document { get; set; }

		ObservableCollection<HistoryItem> collection = new System.Collections.ObjectModel.ObservableCollection<HistoryItem>();

		#endregion

		#region Constructor

		public MainWindow()
		{
			InitializeComponent();
		}

		#endregion

		#region Overrides

		public override void EndInit()
		{
			base.EndInit();

			this.KeyDown += MainWindow_KeyDown;

			this.MedRadioButton.Checked += RadioButton_Checked;
			this.TechRadioButton.Checked += RadioButton_Checked;
			
			this.LoginTextBox.TextChanged += TextChanged;
			this.PasswordTextBox.PasswordChanged += TextChanged;
			this.PerformButton.Click += PerformButton_Click;

			ReadProgramData();

			Closed += (sender, eventArgs) => this.SaveProgramData();
		}

		#endregion

		#region Private methods

		private void FillForm(SettingsEntry entry)
		{
			this.LoginTextBox.Text = entry.Login;

			if (entry.Password != null && entry.Password.Length > 0)
			{
				try
				{
					byte[] passwordUnprotected = ProtectedData.Unprotect(entry.Password, salt, DataProtectionScope.CurrentUser);
					this.PasswordTextBox.Password = Encoding.UTF8.GetString(passwordUnprotected);
				}
				catch(Exception e)
				{
					entry.Password = null;
					MessageBox.Show(e.Message, "Błąd odczytu hasła", MessageBoxButton.OK, MessageBoxImage.Warning);
				}
			}
			else
				this.PasswordTextBox.Password = string.Empty;
		}

		private SettingsEntry ReadForm()
		{
			string login = this.LoginTextBox.Text;
			string password = this.PasswordTextBox.Password;

			byte[] passwordProtected = ProtectedData.Protect(Encoding.UTF8.GetBytes(password), salt, DataProtectionScope.CurrentUser);

			School school = School.Unknown;
			if (this.MedRadioButton.IsChecked == true)
				school = School.Wum;
			else if (this.TechRadioButton.IsChecked == true)
				school = School.Pw;

			return new SettingsEntry()
			{
				Login = login,
				Password = passwordProtected,
				School = school
			};
		}

		private void ReadProgramData()
		{
			try
			{
				if (File.Exists(programDataPath))
				{
					using (StreamReader sr = new StreamReader(programDataPath))
					{
						XmlSerializer xmlSerializer = new XmlSerializer(typeof(ProgramData));
						ProgramData programData = xmlSerializer.Deserialize(sr) as ProgramData;

						if (programData != null)
							Document = programData;
					}
				}
			}
			catch(Exception e)
			{
				MessageBox.Show(e.Message, "Błąd odczytu danych", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			finally
			{
				if (this.Document == null)
				{
					Document = new ProgramData()
					{
						PwSettings = new SettingsEntry() { School = School.Pw },
						WumSettings = new SettingsEntry() { School = School.Wum },
						CurrSchool = School.Pw
					};
				}

				if(Document.CurrSchool == School.Pw)
				{
					this.TechRadioButton.IsChecked = true;
				}
				else if(Document.CurrSchool == School.Wum)
				{
					this.MedRadioButton.IsChecked = true;
				}
			}
		}

		private void SaveProgramData()
		{
			try
			{
				using (StreamWriter wr = new StreamWriter(programDataPath))
				{
					XmlSerializer xmlSerializer = new XmlSerializer(typeof(ProgramData));
					xmlSerializer.Serialize(wr, this.Document);
				}
			}
			catch(Exception e)
			{
				MessageBox.Show(e.Message, "Błąd zapisu danych", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		#endregion

		#region Event handlers

		private void MainWindow_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.Enter:
					SystemSounds.Hand.Play();
					return;
				case Key.Escape:
					Close();
					return;
				default:
					return;
			}
		}

		private void RadioButton_Checked(object sender, RoutedEventArgs e)
		{
			if(sender == this.MedRadioButton && this.MedRadioButton.IsChecked == true)
			{
				this.FillForm(this.Document.WumSettings);
				this.Document.CurrSchool = School.Wum;
			}
			else if(sender == this.TechRadioButton && this.TechRadioButton.IsChecked == true)
			{
				this.FillForm(this.Document.PwSettings);
				this.Document.CurrSchool = School.Pw;
			}
		}

		private void TextChanged(object sender, RoutedEventArgs e)
		{
			if (this.MedRadioButton.IsChecked == true)
			{
				this.Document.WumSettings = ReadForm();
			}
			else if (this.TechRadioButton.IsChecked == true)
			{
				this.Document.PwSettings = ReadForm();
			}
		}

		private void PerformButton_Click(object sender, RoutedEventArgs e)
		{
			
		}

		#endregion

	}
}
