﻿using System;
using AfyaHMIS.Models.Registrations;
using AfyaHMIS.Service;

namespace AfyaHMIS.Models.Finances
{
    public class Bills
    {
        public static IFinanceService IFinanceService = new FinanceService();

        public long Id { get; set; }
        public Visit Visit { get; set; }
        public BillsFlag Flag { get; set; }
        public double Amount { get; set; }
        public double Paid { get; set; }
        public double Waiver { get; set; }
        public string WaiverReason { get; set; }

        public DateTime CreatedOn { get; set; }
        public DateTime? WaivedOn { get; set; }
        public DateTime? ProcessedOn { get; set; }
        public DateTime? WaivedApprovedOn { get; set; }

        public Users CreatedBy { get; set; }
        public Users WaivedBy { get; set; }
        public Users ProcessedBy { get; set; }
        public Users WaivedApprovedBy { get; set; }
        public string Notes { get; set; }

        public Bills()
        {
            Id = 0;
            Visit = new Visit();
            Flag = new BillsFlag();
            Amount = 0;
            Paid = 0;
            Waiver = 0;
            WaiverReason = "";

            CreatedOn = DateTime.Now;

            CreatedBy = new Users();
            WaivedBy = new Users();
            ProcessedBy = new Users();
            WaivedApprovedBy = new Users();
            Notes = "";
        }

        public Bills Save() {
            return IFinanceService.SaveBill(this);
        }

        public Bills UpdateWaiver() {
            return IFinanceService.UpdateBillWaiver(this);
        }

        public Bills UpdateProcess() {
            return IFinanceService.UpdateBillProcess(this);
        }
    }
}
