﻿namespace App.Lib.InstitutionConnection.Exception;

public sealed class InstitutionNotFoundException : System.Exception
{
    public InstitutionNotFoundException(Guid institutionId)
        : base("Institution {InstitutionId} not found")
    {
        Data.Add("InstitutionId", institutionId);
    }
}