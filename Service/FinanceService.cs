using System;
using System.Data.SqlClient;
using AfyaHMIS.Extensions;
using AfyaHMIS.Models.Concepts;
using AfyaHMIS.Models.Finances;

namespace AfyaHMIS.Service
{
	public interface IFinanceService
    {
        public ClientCodeRates GetClientCodeBillableRate(ClientCode code, BillableService service);

        public Bills SaveBill(Bills bill);
        public BillsItem SaveBillsItem(BillsItem item);
    }

	public class FinanceService : IFinanceService 
    {
        public ClientCodeRates GetClientCodeBillableRate(ClientCode code, BillableService service)
        {
            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT bs_idnt, bs_concept, bs_service, bs_amount, bs_description, ISNULL(cr_rate, bs_amount)x FROM BillableService LEFT OUTER JOIN ClientCodesRates ON bs_idnt=cr_service AND cr_code=" + code.Id + " WHERE bs_idnt=" + service.Id);
            if (dr.Read())
                return new ClientCodeRates {
                    Service = new BillableService
                    {
                        Id = Convert.ToInt64(dr[0]),
                        Concept = new Concept { Id = Convert.ToInt64(dr[1])},
                        Name = dr[2].ToString(),
                        Amount = Convert.ToDouble(dr[3]),
                        Description = dr[4].ToString()
                    },
                    Code = code,
                    Amount = Convert.ToDouble(dr[5]),
                };

            return new ClientCodeRates();
        }

        public Bills SaveBill(Bills bill)
        {
            SqlServerConnection conn = new SqlServerConnection();
            bill.Id = conn.SqlServerUpdate("DECLARE @idnt INT=" + bill.Id + ", @visit INT=" + bill.Visit.Id + ", @amount FLOAT=" + bill.Amount + ", @user INT=" + bill.CreatedBy.Id + ", @notes NVARCHAR(MAX)='" + bill.Notes + "'; IF NOT EXISTS (SELECT bl_idnt FROM Bills WHERE bl_idnt=@idnt) BEGIN INSERT INTO Bills (bl_visit, bl_amount, bl_created_by, bl_notes) output INSERTED.bl_idnt VALUES (@visit, @amount, @user, @notes) END ELSE BEGIN UPDATE Bills SET bl_visit=@visit, bl_amount=@amount, bl_notes=@notes output INSERTED.bl_idnt WHERE bl_idnt=@idnt END");

            return bill;
        }

        public BillsItem SaveBillsItem(BillsItem item)
        {
            SqlServerConnection conn = new SqlServerConnection();
            item.Id = conn.SqlServerUpdate("DECLARE @idnt INT=" + item.Id + ", @bill INT=" + item.Bill.Id + ", @service INT=" + item.Service.Id + ", @amount FLOAT=" + item.Amount + ", @user INT=" + item.CreatedBy.Id + ", @desc NVARCHAR(MAX)='" + item.Description + "'; IF NOT EXISTS (SELECT bi_idnt FROM BillsItem WHERE bi_idnt=@idnt) BEGIN INSERT INTO BillsItem (bi_bill, bi_service, bi_amount, bi_created_by, bi_description) output INSERTED.bi_idnt VALUES (@bill, @service, @amount, @user, @desc) END ELSE BEGIN UPDATE BillsItem SET bi_service=@service, bi_amount=@amount, bi_description=@desc output INSERTED.bi_idnt WHERE bi_idnt=@idnt END");

            return item;
        }
    }
}
