using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace PasswordManager.Modules.Utils
{
    public static class LoadingBarUtils
    {
        public static void UpdateProgressBarWithTiming(ProgressBar progressBar, int totalSteps, Window window)
        {
            // Visualizza la barra di progresso
            progressBar.Visibility = Visibility.Visible;

            var startTime = DateTime.Now;

            for (int i = 0; i <= totalSteps; i++)
            {
                // Calcola la percentuale di completamento
                double completionPercentage = (double)i / totalSteps;

                // Aggiorna il valore della barra di progresso in base alla percentuale di completamento
                progressBar.Value = completionPercentage * 100;

                // Calcola il tempo trascorso finora
                var elapsedTime = DateTime.Now - startTime;

                // Calcola il tempo stimato rimanente
                var estimatedRemainingTime = elapsedTime / completionPercentage - elapsedTime;
            }

            // Nascondi la barra di progresso una volta completato il processo
            progressBar.Visibility = Visibility.Hidden;
        }
    }
}
