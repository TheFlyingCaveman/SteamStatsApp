﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace Trfc.SteamStats.ClientServices.GamePictures
{
    public interface IPictureCacheChecker
    {
        Task<CacheResponse> IsCacheOutOfDate(DateTime cacheLastUpdatedTimeUtc, int appId, CancellationToken token);
    }
}