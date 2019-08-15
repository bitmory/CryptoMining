using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoMingingBackend
{
    public interface IWorkerRepository
    {
        //Contact Find(int id);
        //List<Contact> GetAll();
        //Contact Add(Contact contact);
        //Contact Update(Contact contact);
        //void Remove(int id);

        //Contact GetFullContact(int id);
    }


    public class Worker
    {
        public int poolid { get; set; }
        public string workername { get; set; }
        public string currenthashrate { get; set; }
        public string dailyhashrate { get; set; }
        public bool isactive { get; set; }
        public string rejected { get; set; }
        public DateTime updateat { get; set; }
        public string currentcalculation { get; set; }
        public string dailycalculation { get; set; }

    }

        public class Contact
    {
        public Contact()
        {
            this.Addresses = new List<Address>();
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Company { get; set; }
        public string Title { get; set; }

        public List<Address> Addresses { get; private set; }

        public bool IsNew
        {
            get
            {
                return this.Id == default(int);
            }
        }
    }
    public class Address
    {
        public int Id { get; set; }
        public int ContactId { get; set; }
        public string AddressType { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public int StateId { get; set; }
        public string PostalCode { get; set; }

        internal bool IsNew
        {
            get
            {
                return this.Id == default(int);
            }
        }

        public bool IsDeleted { get; set; }
    }
    public class State
    {
        public int Id { get; set; }
        public string StateName { get; set; }
    }
}
