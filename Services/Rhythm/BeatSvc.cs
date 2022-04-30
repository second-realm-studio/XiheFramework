using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XiheFramework;

public static class BeatSvc {
    public static float GetTimer() => Game.Beat.Timer;
    public static float GetBPM() => Game.Beat.bpm;
    public static float GetTPB() => Game.Beat.TimePerBeat;
}