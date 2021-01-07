using System.Security.Principal;
using Microsoft.Extensions.Logging;

namespace APS.Methods.Profit
{
    public static class Upload
    {

        internal static void Save(string fileName, IPrincipal User, ILogger _logger)
        {
            BusinessLogic.ImportLog.InputDialog(fileName);
        }
        //    internal static void Save(IEnumerable<IFormFile> files, IPrincipal User, ILogger _logger)
        //    {
        //        if (files != null)
        //        {
        //            foreach (var file in files)
        //            {
        //                var fileContent = ContentDispositionHeaderValue.Parse(file.ContentDisposition);

        //                // Some browsers send file names with full path.
        //                // We are only interested in the file name.
        //                var fileName = Path.GetFileName(fileContent.FileName.ToString().Trim('"'));

        //                BusinessLogic.ImportLog.InputDialog(fileName);


        //                //    //todo one approach is to map the Root of the file to a tree of objects that match, then filter the relevant into and save to db
        //                //using (StreamReader r = new StreamReader(file.OpenReadStream()))
        //                //{
        //                //    //string json = r.ReadToEnd();
        //                //    //var items = JsonConvert.DeserializeObject<Root>(json);
        //                //    //sum = items.where.sum().etc
        //                //    //SaveToDb(sum);

        //                //}
        //                _logger.LogInformation("creating file for Profit." + JsonConvert.SerializeObject(fileName) + " by " + User.Identity.Name);
        //            }
        //        }
        //    }

        //todo
        //private static void SaveToDb(List<BusinessLogic.Models.TradeOperation> sum)
        //{
        //    using (DBContext db = new DBContext())
        //    {
        //        db.TradeOperation.RemoveRange(db.v.Where(x=>x.Date>= firstDay && x.Date<=lastDay && x.User == user));
        //        db.TradeOperation.AddRange(sumdb);

        //        db.SaveChanges();
        //    }
        //}

        //public static void ClearData(ClaimsPrincipal user)
        //{
        //    using (DBContext db = new DBContext())
        //    {
        //        db.TradeOperation.RemoveRange(db.TradeOperation.Where(x=>x.User == user.Identity.Name));

        //        db.SaveChanges();
        //    }
        //}
    }
}
