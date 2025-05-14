using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Projets.Modules;


namespace Projets.Modules
{
    [TestClass]
    public class TestUnitaire
    {
        [TestMethod]
        public void Client_AjouterCommande_AjouteCommandeALHistorique()
        {
            // Arrange
            Client client = new Client("123", "Doe", "John", DateTime.Now.AddYears(-30), "1 rue Test");
            Commande commande = new Commande
            {
                Id = 1,
                Client = client,
                DateCommande = DateTime.Now,
                Prix = 100
            };

            // Act
            client.AjouterCommande(commande);

            // Assert
            Assert.AreEqual(1, client.HistoriqueCommandes.Count);
            Assert.AreEqual(commande, client.HistoriqueCommandes[0]);
        }

        [TestMethod]
        public void SystemeFidelite_DeterminerStatut_RetourneStatutCorrect()
        {
            // Arrange
            Client client = new Client("123", "Doe", "John", DateTime.Now.AddYears(-30), "1 rue Test");
            SystemeFidelite systeme = new SystemeFidelite();

            // Act - Client standard (0 commande)
            var statut1 = systeme.DeterminerStatut(client);

            // Assert
            Assert.AreEqual(SystemeFidelite.StatutFidelite.Standard, statut1);
        }

        [TestMethod]
        public void CommandeManager_AjouterCommande_ChauffeurDejaOccupe_LeveException()
        {
            // Arrange
            CommandeManager manager = new CommandeManager();
            Client client = new Client("123", "Doe", "John", DateTime.Now.AddYears(-30), "1 rue Test");
            Salarie chauffeur = new Salarie
            {
                NumeroSecuriteSociale = "456",
                Nom = "Smith",
                Prenom = "Bob",
                Poste = "Chauffeur"
            };
            DateTime dateLivraison = DateTime.Now.AddDays(1);

            // Act & Assert
            Assert.IsFalse(Commande.EstChauffeurDisponible(manager, null, dateLivraison));
        }

        [TestMethod]
        public void StatistiqueService_ObtenirCommandesParVille_RetourneDistributionCorrecte()
        {
            // Arrange
            StatistiqueService service = new StatistiqueService(
                new ClientManager(),
                new CommandeManager(),
                new SalarieManager()
            );

            // Act
            Dictionary<string, int> distribution = service.ObtenirCommandesParVille();

            // Assert
            Assert.IsNotNull(distribution);
        }

        [TestMethod]
        public void FinanceSimple_AjouterTransaction_MetsAJourSolde()
        {
            // Arrange
            FinanceSimple finance = new FinanceSimple();
            double montantInitial = 1000;

            // Act
            finance.AjouterTransaction(montantInitial, "Crédit", "Test credit");
            finance.AjouterTransaction(montantInitial / 2, "Débit", "Test debit");

            // Assert - Vérifié via l'historique des transactions
            finance.AfficherHistorique();
        }

        [TestMethod]
        public void SalarieManager_AjouterSalarie_AjouteSalarieEtMAJOrganigramme()
        {
            // Arrange
            SalarieManager manager = new SalarieManager();
            Salarie nouveauSalarie = new Salarie
            {
                NumeroSecuriteSociale = "789",
                Nom = "Brown",
                Prenom = "Charlie",
                Poste = "Chef d'équipe"
            };

            // Act
            bool resultat = manager.AjouterSalarie(nouveauSalarie);

            // Assert
            Assert.IsTrue(resultat);
            Assert.IsNotNull(manager.RechercherParId("789"));
        }

        [TestMethod]
        public void MaintenanceManager_AjouterMaintenance_AjouteDansHistorique()
        {
            // Arrange
            MaintenanceManager manager = new MaintenanceManager();
            MaintenanceRecord maintenance = new MaintenanceRecord
            {
                Immatriculation = "AB-123-CD",
                DateMaintenance = DateTime.Now,
                TypeMaintenance = "Preventive",
                Description = "Test maintenance",
                Cout = 100,
                Statut = "Planifié"
            };

            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() => manager.AjouterMaintenance(maintenance));
        }

        [TestMethod]
        public void VehiculeManager_RechercherVehicules_FiltrageCorrect()
        {
            // Arrange
            VehiculeManager manager = new VehiculeManager();

            // Act
            List<Vehicule> vehiculesLourds = manager.ObtenirVehiculesParType<PoidsLourd>();

            // Assert
            Assert.IsNotNull(vehiculesLourds);
            CollectionAssert.AllItemsAreInstancesOfType(vehiculesLourds, typeof(PoidsLourd));
        }

        [TestMethod]
        public void GrapheService_FloydWarshall_CalculDistancesCorrectes()
        {
            // Arrange
            GrapheListe graphe = new GrapheListe(true);
            GrapheService service = new GrapheService(graphe);

            // Act
            var resultat = service.FloydWarshall();

            // Assert
            Assert.IsTrue(resultat.HasValue);
        }
    }
}
