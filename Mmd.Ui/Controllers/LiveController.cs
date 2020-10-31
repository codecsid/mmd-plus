using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using mmd_plus.Models;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Threading;
using DataAccess.Data.Abstract;
using DataAccess.Model;
using DataAccess.Model.SharedModels;
using Microsoft.Extensions.Logging;


namespace mmd_plus.Controllers
{
    public class LiveController : Controller
    {

        private readonly ILiveService _liveService;

        public LiveController(ILiveService liveService)
        {
            _liveService = liveService;
        }

        public LiveViewModel getLiveViewModel(LiveResponse liveResponse) { 
            return new LiveViewModel
            {
                GameId = liveResponse.GameId,
                RoundId = liveResponse.RoundId,
                RoundNumber = liveResponse.RoundNumber,
                Phase = liveResponse.Phase,
                SecretLength = liveResponse.SecretLength,
                JoiningDuration = liveResponse.JoiningDuration,
                RunningDuration = liveResponse.RunningDuration,
                FinishedDuration = liveResponse.FinishedDuration,
                PhaseStartTime = liveResponse.PhaseStartTime,
                Participants_CurrentScore = liveResponse.Participants_Current.OrderByDescending(o => o.Score).ToList(),
                Participants_TotalScore = liveResponse.Participants_Total.OrderByDescending(o => o.Score).ToList()

            };
        }

        public IActionResult Index()
        {

            return View();

        }
       

        public async Task<IActionResult> LoadData() {
            LiveResponse liveResponse = await _liveService.GetLiveStatus();

            return Json(getLiveViewModel(liveResponse));

        }

    }
}
