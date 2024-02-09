using POSAccounting.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSAccounting.Models
{
    public class InvoiceLineM : ViewModelBase
    {
        private Guid id;
        private int qty;
        private decimal price;
        private string notes;
        private Guid invoiceId;
        private Guid productUnitId;
        private ProductUnitM productUnit;
        private InvoiceM invoice;
        private decimal total;
        private ProductM product;
        private ObservableCollection<SimpleM> strings = new ObservableCollection<SimpleM>() { new SimpleM { Id = 1, Name = "aa" } };

        public Guid Id { get { return id; } set { id = value; NotifyPropertyChanged("Id"); } }
        public int QTY { get { return qty; } set { TotalPrice = value * Price; qty = value; NotifyPropertyChanged("QTY"); } }
        public decimal Price { get { return price; } set { TotalPrice = value * QTY; price = value; NotifyPropertyChanged("Price"); } }
        public string Notes { get { return notes; } set { notes = value; NotifyPropertyChanged("Notes"); } }
        public Guid InvoiceId { get { return invoiceId; } set { invoiceId = value; NotifyPropertyChanged("InvoiceId"); } }
        public Guid ProductUnitId { get { return productUnitId; } set { productUnitId = value; NotifyPropertyChanged("ProductUnitId"); } }
        public ProductUnitM ProductUnit { get { return productUnit; } set { productUnit = value; NotifyPropertyChanged("ProductUnit"); } }
        public InvoiceM Invoice { get { return invoice; } set { invoice = value; NotifyPropertyChanged("Invoice"); } }
        public decimal TotalPrice { get { return total; } set { total = value; NotifyPropertyChanged("TotalPrice"); } }
        public ProductM Product { get { return product; } set { product = value; NotifyPropertyChanged("Product"); } }
        public ObservableCollection<SimpleM> Strings { get { return strings; } set { strings = value; NotifyPropertyChanged("Strings"); } }

    }

    public class InvoiceLineUtils
    {
        public static InvoiceLineM FromEntity(InvoiceLine entity)
        {
            var model = entity == null ? null :
                new InvoiceLineM()
                {
                    Id = entity.Id,
                    QTY = entity.QTY,
                    Price = entity.Price,
                    Notes = entity.Notes,
                    InvoiceId = entity.InvoiceId,
                    ProductUnitId = entity.ProductUnitId,
                };
            return model;
        }

        public static InvoiceLine FromModel(InvoiceLineM model)
        {
            return model == null ? null :
                new InvoiceLine()
                {
                    Id = model.Id,
                    QTY = model.QTY,
                    Price = model.Price,
                    Notes = model.Notes,
                    InvoiceId = model.InvoiceId,
                    ProductUnitId = model.ProductUnitId,
                };
        }
    }
}
