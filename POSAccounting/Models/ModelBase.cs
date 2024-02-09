using POSAccounting.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace POSAccounting.Models
{
    public class IntM : ViewModelBase
    {
        private int id;
        public int Id { get { return id; } set { id = value; NotifyPropertyChanged("Id"); } }
    }

    public class SimpleM : IntM
    {
        public string name;
        public string Name { get { return name; } set { name = value; NotifyPropertyChanged("Name"); } }
    }

    public class SimpleShortM : ViewModelBase
    {
        private short id;
        public string name;
        public short Id { get { return id; } set { id = value; NotifyPropertyChanged("Id"); } }
        public string Name { get { return name; } set { name = value; NotifyPropertyChanged("Name"); } }

    }

    public class SimpleByteM : ViewModelBase
    {
        private byte id;
        private string name;
        public byte Id { get { return id; } set { id = value; NotifyPropertyChanged("Id"); } }
        public string Name { get { return name; } set { name = value; NotifyPropertyChanged("Name"); } }
    }

    public class ImgResultM
    {
        public string OrignalPath { get; set; }
        public string NewPath { get; set; }
    }
}
