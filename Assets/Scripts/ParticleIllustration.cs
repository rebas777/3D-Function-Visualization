using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleIllustration {
    public ParticleSystem system;
    ParticleSystem.Particle[] m_Particles;
    public Vector3 hide = new Vector3(100, 100, 100);

    public float ScaleSize=0.075f;

    int countp = 0;

    public ParticleIllustration(ParticleSystem sys) {
        system = sys;
      //  system.transform.position = hide;
        var mainModule = system.main;
        mainModule.startColor = Color.yellow;
        mainModule.startSize = 0.01f;
        mainModule.maxParticles = 100000;

        var emitParams = new ParticleSystem.EmitParams();
        system.Emit(emitParams, system.main.maxParticles);

        m_Particles = new ParticleSystem.Particle[100000];
        initialParticle();
        //drawParticle(hide);
        endDraw();

    }
    public ParticleIllustration(ParticleSystem sys,Color c)
    {
        system = sys;
        system.transform.position = hide;
        var mainModule = system.main;
        mainModule.startColor = c;
        mainModule.startSize = 1f;
        mainModule.maxParticles = 100000;

        var emitParams = new ParticleSystem.EmitParams();
        system.Emit(emitParams, system.main.maxParticles);

        m_Particles = new ParticleSystem.Particle[100000];
        initialParticle();
        drawParticle(hide);
        endDraw();
    }

    public void initialParticle() {
        system.GetParticles(m_Particles);
        Debug.Log(m_Particles.Length);
        countp = 0;
    }

    public void drawParticle(Vector3 posi) {
        system.transform.localPosition = Vector3.zero;
        if (countp < system.main.maxParticles)
        {
            
            m_Particles[countp].position = ScaleSize* posi;//设置他们的位置
 
            countp++;
        }
    }

    public void endDraw() {
        system.SetParticles(m_Particles, countp);
    }


    
}
