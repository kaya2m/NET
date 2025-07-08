using System;
using System.Collections.Generic;
using System.Linq;

namespace NET.Application.Common.Models
{
    public class Result
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public List<string> Errors { get; set; } = new();

        public static Result Success(string? message = null)
        {
            return new Result { IsSuccess = true, Message = message };
        }

        public static Result Failure(string error)
        {
            return new Result { IsSuccess = false, Errors = new List<string> { error } };
        }

        public static Result Failure(IEnumerable<string> errors)
        {
            return new Result { IsSuccess = false, Errors = errors.ToList() };
        }
    }

    public class Result<T> : Result
    {
        public T? Data { get; set; }

        public static Result<T> Success(T data, string? message = null)
        {
            return new Result<T> { IsSuccess = true, Data = data, Message = message };
        }

        public static new Result<T> Failure(string error)
        {
            return new Result<T> { IsSuccess = false, Errors = new List<string> { error } };
        }

        public static new Result<T> Failure(IEnumerable<string> errors)
        {
            return new Result<T> { IsSuccess = false, Errors = errors.ToList() };
        }
    }
}