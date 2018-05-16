//Robert Mccormick
//Frameworkds
//Term3
//RobertMcCormick_CE07


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MccormickRobert_JSONandAPI.mods;
using System.Xml;
using System.Xml.Serialization;




namespace MccormickRobert_JSONandAPI
{
    public partial class Form1 : Form
    {
        //my unique key for validate input texct file
      
        private const string KEY = "My Program";

        WebClient apiConnection = new WebClient();

        private Weather weather;

        string apiStartingPoint = "http://api.wunderground.com/api/b513344ce4d46def/conditions/q/";

        //API starting point
        // current observations
        //API ending point 
        string apiEndPoint;


        public Form1()
        {
            InitializeComponent();

            weather = new Weather();
        }

        //exit app
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }



        //state abbrev stored into list for selection
        private string ReturnStateAbbrev()
        {
            string abbrev = "";
            string[] stateAbbrev = new string[] { "AL", "AK", "AZ", "AR", "CA", "CO", "CT", "DE", "FL", "GA", "HI", "ID", "IL", "IN", "IA", "KS", "KY", "LA", "ME", "MD", "MA", "MI", "MN", "MS", "MO", "MT", "NE", "NV", "NH", "NJ", "NM", "NY", "NC", "ND", "OH", "OK", "OR", "PA", "RI", "SC", "SD", "TN", "TX", "UT", "VT", "VA", "WA", "WV", "WI", "WY" };

            //match the state to the abbreviation 
            abbrev = stateAbbrev[cmbState.SelectedIndex];

            return abbrev;
        }



        //api from website
        private void BuildAPI()
        {
            //Get the state abbreviation 
            string abbrev = ReturnStateAbbrev();

            //replace any spaces in the city 
            string city = txtCity.Text.Replace(' ', '_');

               string json = ".json";
        

            //complete the API 
            apiEndPoint = apiStartingPoint + abbrev + "/" + city + json;   // city + ".json";

            // output the API string to a textbox 
           // textBox1.Text = apiEndPoint;
        }


       //get data
        private void btnViewWeatherData_Click(object sender, EventArgs e)
        {
             
                try
                {
                    BuildAPI();

                    //call the correct method             
                        ReadTheJSON();
                 //ReadTheData();
                    ApplyView(weather);
                }

                catch (Exception ex)
                {
                    MessageBox.Show("No Internet connection!");
                }

            }
         

       

      

        // read JSON
        private void ReadTheJSON() //ReadTheData()
        {
  
            var apiData = apiConnection.DownloadString(apiEndPoint);

            JObject o = JObject.Parse(apiData);


            string specifics = o["current_observation"]["display_location"]["full"].ToString();
            // see the object as a string  
            MessageBox.Show(specifics);   //"The JObject: \n" + o.ToString())
            //read response
            weather = new Weather();

            
            weather.CurrTemp = Convert.ToDecimal(o["current_observation"]["temp_f"]);
            weather.FeelsTemp = Convert.ToDecimal(o["current_observation"]["feelslike_f"]);
            string relativeHumidity = o["current_observation"]["relative_humidity"].ToString();
            weather.RelativeHumidity = Convert.ToDecimal(relativeHumidity.Substring(0, relativeHumidity.Length - 1));
            weather.WindSpeed = Convert.ToDecimal(o["current_observation"]["wind_mph"]);
            weather.Direction = o["current_observation"]["wind_dir"].ToString();
            weather.City = o["current_observation"]["display_location"]["city"].ToString();
            weather.State = o["current_observation"]["display_location"]["state"].ToString();

            ApplyView(weather);
            ApplyModel();

        }


        //store input values from json or xml on net on txt
        private void ApplyView(Weather model)
        {

            numTemp.Value = model.CurrTemp;
            txtDirection.Text = model.Direction;
            numFeelsLikeTemp.Value = model.FeelsTemp;
            numRelativeHumidity.Value = model.RelativeHumidity;
            numWindSpeed.Value = model.WindSpeed;
            txtCity.Text = model.City;
            cmbState.Text = model.State;
        }

        //load value into textbox

        private void ApplyModel()
        {
            weather.UniqueKey = KEY;

            weather.Direction = txtDirection.Text;
            weather.FeelsTemp = numFeelsLikeTemp.Value;
            weather.RelativeHumidity = numRelativeHumidity.Value;
            weather.CurrTemp = numTemp.Value;
            weather.WindSpeed = numWindSpeed.Value;
            weather.City = txtCity.Text;  


             // textBox1.Text = (weather.City + ", " + weather.State);


        }


       // parse
        private string GetExt(string filename)
        {
            var temp = filename.Split('.');
            return temp[temp.Length - 1];
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {



            //Stream myStream;
            // SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            ApplyModel();

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {


                saveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                saveFileDialog1.FilterIndex = 1;
                saveFileDialog1.RestoreDirectory = true;

                File.WriteAllText(saveFileDialog1.FileName, JsonConvert.SerializeObject(weather));
                MessageBox.Show("Saved!");
            }
        }

                
             
         
        // load file
        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //load data from file


            try
            {

                if (openFileDialog1.ShowDialog() == DialogResult.OK)

                {

                    if (GetExt(openFileDialog1.FileName).ToLower().Equals("txt"))
                    {

                        weather = JsonConvert.DeserializeObject<Weather>(File.ReadAllText(openFileDialog1.FileName));

                    }

                    //check KEY first
                    if (!KEY.Equals(weather.UniqueKey))
                    {
                        MessageBox.Show("Not valid text file!");
                        return;
                    }

                    ApplyView(weather);
                    MessageBox.Show("Loaded");

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Data file incorrect!");
            }


        }

        // new file
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            weather = new Weather();
            ApplyView(weather);
        }

        
    }
}


 