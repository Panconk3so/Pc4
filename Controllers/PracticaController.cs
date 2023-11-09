using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ML;

namespace Practica.Controllers
{

    public class PracticaController : Controller
    {
        private readonly ILogger<PracticaController> _logger;
        private readonly PredictionEnginePool<practica.ModelInput, practica.ModelOutput> _predictionEnginePool;

        public PracticaController(ILogger<PracticaController> logger, PredictionEnginePool<practica.ModelInput, practica.ModelOutput> predictionEnginePool)
        {
            _logger = logger;
            _predictionEnginePool = predictionEnginePool;
        }

        public IActionResult Index()
        
        {
            return View("Views/Practica/Index.cshtml");
        }

        [HttpPost]
        public IActionResult Comentario(string comentario)
        {
            var input = new practica.ModelInput
            {
                Col0 = comentario
            };


           practica.ModelOutput prediction = _predictionEnginePool.Predict(input);


            ViewBag.Resultado = prediction.PredictedLabel;

             return View("Views/Practica/Evaluacion.cshtml");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error");
        }
    }
}