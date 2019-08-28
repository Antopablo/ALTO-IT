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
    public class FormatageTest
    {
        Dashboard D = new Dashboard(new MainWindow(), new Projet("Test", PROVIDER.AZURE));

        [TestMethod()]
        public void TableFormater_WithValue_Hello_ID15_Output__15Hello()
        {
            D.NormeSelectionnee = new Norme("NormeTest", 1);
            D.NormeSelectionnee.Id = 15;
            Assert.AreEqual("_15Hello", D.TableFormater("Hello"));
        }

        [TestMethod()]
        public void FormaterToSQLRequestTest()
        {
            Assert.AreEqual("$$Hello", D.FormaterToSQLRequest("/'=Hello"));
        }

        [TestMethod()]
        public void SimpleQuoteFormaterTest()
        {
            Assert.AreEqual("c''est moi", D.SimpleQuoteFormater("c'est moi"));
        }

        [TestMethod()]
        public void MultipleFormatageTest()
        {
            D.NormeSelectionnee = new Norme("NormeTest", 1);
            D.NormeSelectionnee.Id = 99;
            Assert.AreEqual("_99$$$_$Cest___moi_____$$$_____", D.TableFormater(D.SimpleQuoteFormater(D.FormaterToSQLRequest("$== (C'est _ moi)°°) ==$     "))));
        }

    }
}
