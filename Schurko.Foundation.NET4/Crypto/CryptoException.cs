﻿
using System;



namespace Schurko.Foundation.Crypto
{
  public class CryptoException : Exception
  {
    public CryptoException(string message)
      : base(message)
    {
    }
  }
}
