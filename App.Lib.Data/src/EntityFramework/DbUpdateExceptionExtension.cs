using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace App.Lib.Data.EntityFramework;

public static class DbUpdateExceptionExtension
{
    public static bool IsUniqueConstraintViolation(this DbUpdateException exception)
    {
        if (exception.InnerException is SqlException sqlException)
        {
            return sqlException.Number == 2601;
        }
        return false;
    }
}