using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkoutOptimization.Logic.Interfaces;
using WorkoutOptimization.Models.Entities;
using WorkoutOptimization.Models.Models;

namespace WorkoutOptimization.Logic.Classes
{
    public class PulseLogic : IPulseLogic
    {
        public PulseViewModel CompareToRestPulse(User user, int pulse)
        {
            if(user.RestPulse==null || user.RestPulse == 0)
            {
                throw new Exception("User need to setup the restpulse");
            }
            int hrMax = 220;
            int pulseMax = hrMax - user.GetAge;
            double startTrainingPulsePercentage = 0.5;
            int res = (int)(user.RestPulse + startTrainingPulsePercentage * (pulseMax - user.RestPulse));
            if (res+14 > pulse)
            {
                return new PulseViewModel() { Message = "Megfelelő a pulzusod!", Pulse = pulse };
            }
            else
            {
                return new PulseViewModel() { Message = "A pulzusod magas!", Pulse = pulse };
            }
        }
    }
}
