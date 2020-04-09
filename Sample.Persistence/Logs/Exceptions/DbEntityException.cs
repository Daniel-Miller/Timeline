using System;
using System.Data.Entity.Validation;
using System.Text;

namespace Sample.Persistence.Logs
{
    internal class DbEntityException : Exception
    {
        public DbEntityException(Exception inner) : base(null, inner) { }

        public DbEntityException(string s, Exception exception) : base(s, exception) { }

        public override string Message
        {
            get
            {
                var inner = InnerException as DbEntityValidationException;
                if (inner == null) return base.Message;

                var sb = new StringBuilder();

                sb.AppendLine();
                sb.AppendLine();

                foreach (var x in inner.EntityValidationErrors)
                {
                    sb.AppendLine($"- \"{x.Entry.Entity.GetType().FullName}\" Entity ({x.Entry.State}) has validation errors:");
                    foreach (var y in x.ValidationErrors)
                    {
                        sb.AppendLine($"-- Property: \"{y.PropertyName}\", Value: \"{x.Entry.CurrentValues.GetValue<object>(y.PropertyName)}\", Error: \"{y.ErrorMessage}\"");
                    }
                }
                sb.AppendLine();

                return sb.ToString();
            }
        }
    }
}
