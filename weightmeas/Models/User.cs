using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using DataAnnotationsExtensions;

namespace weightmeas.Models
{
    public class User
    {
        private string _password = "";
        private Collection<WeightPlot> weightPlots;

        [Key]
        [Email]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password
        {
            get { return _password; }
            set { _password = HashPassword(value); }
        }

        [StringLength(10, MinimumLength = 10)]
        [Required]
        public string PrivateToken { get; set; }

        public virtual ICollection<WeightPlot> WeightPlots
        {
            get { return weightPlots ?? (weightPlots = new Collection<WeightPlot>()); }
        } 

        public static string HashPassword(string clearTextPassword)
        {
            var crypto = new System.Security.Cryptography.MD5CryptoServiceProvider();
            var data = System.Text.Encoding.ASCII.GetBytes(clearTextPassword);
            data = crypto.ComputeHash(data);
            var md5Hash = System.Text.Encoding.ASCII.GetString(data);
            return md5Hash;
        }
    }
}