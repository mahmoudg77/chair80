using Newtonsoft.Json;
using System;
using System.Collections.Generic;


namespace Chair80.BLL
{
   
    interface ITranslatable
    {
        [JsonIgnore]
         bool GetMasterField { get; set; }
        string translate(string master);
    }
    public class  Translatable:ITranslatable
    {
       
        public bool GetMasterField { get; set; }

        public string translate(string master)
        {
            try
            {
                if (GetMasterField) return master;
                var obj = JsonConvert.DeserializeObject<Dictionary<string, string>>(master);
                string lang = GlobalRequestData.lang;
                return obj[lang];
            }
            catch (Exception)
            {

                return master;
            }
        }
    }
    public class CountryTranslate:Translatable
    {

        private string _name;
       
        public string name {
            get {

                return translate(_name);
                

            }set {
                this._name = value;
            }
        }

    }

    public class CityTranslate : Translatable
    {

        private string _name;

        public string name
        {
            get
            {
       
                return translate(_name);

            }
            set
            {
                this._name = value;
            }
        }

    }
    public class GenderTranslate : Translatable
    {

        private string _name;

        public string name
        {
            get
            {
       
                return translate(_name);

            }
            set
            {
                this._name = value;
            }
        }

    }

    public class vwProfileTranslate : Translatable
    {

       
        private string _city_name;
        private string _gender_name;
        private string _country_name;

        public string gender_name
        {
            get
            {

                return translate(_gender_name);

            }
            set
            {
                this._gender_name = value;
            }
        }
        public string country_name
        {
            get
            {

                return translate(_country_name);

            }
            set
            {
                this._country_name = value;
            }
        }
        public string city_name
        {
            get
            {

                return translate(_city_name);

            }
            set
            {
                this._city_name = value;
            }
        }

    }

    public class vwTripsDetailsTranslate : Translatable
    {
        private string _acc_gender { get; set; }
        private string _acc_city { get; set; }
        private string _acc_country { get; set; }
        //private string _trip_gender_name { get; set; }
        private string _trip_type { get; set; }

        public string trip_type
        {
            get
            {

                return translate(_trip_type);

            }
            set
            {
                this._trip_type = value;
            }
        }
        //public string trip_gender_name
        //{
        //    get
        //    {

        //        return translate(_trip_gender_name);

        //    }
        //    set
        //    {
        //        this._trip_gender_name = value;
        //    }
        //}
        public string acc_gender
        {
            get
            {

                return translate(_acc_gender);

            }
            set
            {
                this._acc_gender = value;
            }
        }
        public string acc_country
        {
            get
            {

                return translate(_acc_country);

            }
            set
            {
                this._acc_country = value;
            }
        }
        public string acc_city
        {
            get
            {

                return translate(_acc_city);

            }
            set
            {
                this._acc_city = value;
            }
        }

    }

    public class TripTypesTranslate : Translatable
    {

        private string _name;

        public string name
        {
            get
            {

                return translate(_name);


            }
            set
            {
                this._name = value;
            }
        }

    }
}