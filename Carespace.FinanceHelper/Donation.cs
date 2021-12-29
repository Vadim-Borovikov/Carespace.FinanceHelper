using System;
using System.Collections.Generic;
using Carespace.FinanceHelper.Dto.PayMaster;
using GoogleSheetsManager;

namespace Carespace.FinanceHelper
{
    public sealed class Donation : ILoadable, ISavable
    {
        IList<string> ISavable.Titles => Titles;

        public DateTime Date { get; private set; }

        internal int PaymentId { get; private set; }
        internal decimal Amount { get; private set; }
        internal Transaction.PayMethod? PayMethodInfo { get; private set; }
        internal decimal Total { private get; set; }

        private string _name;

        public Donation() { }

        internal Donation(ListPaymentsFilterResult.Response.Payment payment)
        {
            _name = payment.SiteInvoiceId;
            Date = payment.LastUpdateTime;
            Amount = payment.PaymentAmount;

            switch (payment.PaymentSystemId)
            {
                case 161:
                    PayMethodInfo = Transaction.PayMethod.Sbp;
                    break;
                case 162:
                    PayMethodInfo = Transaction.PayMethod.BankCard;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            PaymentId = payment.PaymentId;
        }

        public void Load(IDictionary<string, object> valueSet)
        {
            PaymentId = valueSet[PaymentIdTitle]?.ToInt() ?? throw new ArgumentNullException("Empty id");

            _name = valueSet[NameTitle]?.ToString();

            Date = valueSet[DateTitle]?.ToDateTime() ?? throw new ArgumentNullException($"Empty date in ${PaymentId}");

            Amount = valueSet[AmountTitle]?.ToDecimal() ?? throw new ArgumentNullException($"Empty amount in ${PaymentId}");

            PayMethodInfo = valueSet.ContainsKey(PayMethodInfoTitle) ? valueSet[PayMethodInfoTitle]?.ToPayMathod() : null;

            Total = valueSet[TotalTitle]?.ToDecimal() ?? throw new ArgumentNullException($"Empty total in ${PaymentId}");
        }

        public IDictionary<string, object> Save()
        {
            return new Dictionary<string, object>
            {
                { NameTitle, _name ?? "" },
                { DateTitle, $"{Date:d MMMM yyyy}" },
                { AmountTitle, Amount },
                { PayMethodInfoTitle, PayMethodInfo.ToString() },
                { PaymentIdTitle, Utils.GetPayMasterHyperlink(PaymentId) },
                { TotalTitle, Total }
            };
        }

        private static readonly List<string> Titles = new List<string>
        {
            NameTitle,
            DateTitle,
            AmountTitle,
            PayMethodInfoTitle,
            PaymentIdTitle,
            TotalTitle
        };

        private const string NameTitle = "От кого";
        private const string DateTitle = "Дата";
        private const string AmountTitle = "Сумма";
        private const string PayMethodInfoTitle = "Способ";
        private const string PaymentIdTitle = "Поступление";
        private const string TotalTitle = "Итого";
    }
}
