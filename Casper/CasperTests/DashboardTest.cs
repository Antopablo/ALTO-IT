
using Alto_IT;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CasperTests
{
    [TestClass()]
    public class DashboardTest
    {
        MainWindow m = new MainWindow();
        Projet P = new Projet("Test", PROVIDER.AWS);
        Norme SelectedNorme = new Norme("Norme_test", 1);

        [TestMethod()]
        public void DashbordTest_WithValueMesures_Normes()
        {
            Dashboard D = new Dashboard(m, P);
            Assert.AreEqual("Mesures", D.ROOT_Mesures.Name);
            Assert.AreEqual("Normes", D.ROOT_Normes.Nom_Norme);
        }
    }
}
