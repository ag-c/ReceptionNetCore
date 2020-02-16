﻿namespace Reception.Model.Network
{
    public interface IQueryResult<T>
    {
        public T Data { get; set; }
        public ErrorCode ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }

    public enum ErrorCode
    {
        Ok,
        NotFound
    }
}