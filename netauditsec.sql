-- MariaDB dump 10.19  Distrib 10.4.21-MariaDB, for Win64 (AMD64)
--
-- Host: localhost    Database: netauditsec
-- ------------------------------------------------------
-- Server version	10.4.21-MariaDB

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `active_devices`
--

DROP TABLE IF EXISTS `active_devices`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `active_devices` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `ip` varchar(20) NOT NULL,
  `id_scan` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `id_scan` (`id_scan`),
  CONSTRAINT `active_devices_ibfk_1` FOREIGN KEY (`id_scan`) REFERENCES `network_scans` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=57 DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `active_devices`
--

LOCK TABLES `active_devices` WRITE;
/*!40000 ALTER TABLE `active_devices` DISABLE KEYS */;
INSERT INTO `active_devices` VALUES (3,'192.168.100.1',6),(4,'192.168.100.151',6),(5,'192.168.100.1',7),(6,'192.168.100.151',7),(7,'192.168.100.120',7),(8,'192.168.100.1',8),(9,'192.168.100.144',8),(10,'192.168.100.150',8),(11,'192.168.100.151',8),(12,'192.168.100.208',8),(13,'192.168.100.208',9),(14,'192.168.100.1',10),(15,'192.168.100.144',10),(16,'192.168.100.150',10),(17,'192.168.100.151',10),(18,'192.168.100.208',10),(51,'192.168.100.1',16),(52,'192.168.100.144',16),(53,'192.168.100.150',16),(54,'192.168.100.151',16),(55,'192.168.100.154',16),(56,'192.168.100.178',16);
/*!40000 ALTER TABLE `active_devices` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `network_scans`
--

DROP TABLE IF EXISTS `network_scans`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `network_scans` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `date` datetime NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=17 DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `network_scans`
--

LOCK TABLES `network_scans` WRITE;
/*!40000 ALTER TABLE `network_scans` DISABLE KEYS */;
INSERT INTO `network_scans` VALUES (6,'2024-01-23 18:34:28'),(7,'2024-01-23 18:39:55'),(8,'2024-01-23 18:46:39'),(9,'2024-01-22 18:46:39'),(10,'2024-01-23 22:56:09'),(16,'2024-01-24 15:50:30');
/*!40000 ALTER TABLE `network_scans` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `open_ports`
--

DROP TABLE IF EXISTS `open_ports`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `open_ports` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `port` int(11) NOT NULL,
  `id_scan` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `open_ports_ibfk_1_idx` (`id_scan`),
  CONSTRAINT `open_ports_ibfk_1` FOREIGN KEY (`id_scan`) REFERENCES `port_scans` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `open_ports`
--

LOCK TABLES `open_ports` WRITE;
/*!40000 ALTER TABLE `open_ports` DISABLE KEYS */;
INSERT INTO `open_ports` VALUES (1,80,1),(2,22,2),(3,25,2),(4,22,3),(5,25,3),(6,53,3),(7,80,3),(8,110,3),(9,119,3),(10,143,3),(11,443,3),(12,995,3);
/*!40000 ALTER TABLE `open_ports` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `port_scans`
--

DROP TABLE IF EXISTS `port_scans`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `port_scans` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `date` datetime NOT NULL,
  `ip` varchar(20) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `port_scans`
--

LOCK TABLES `port_scans` WRITE;
/*!40000 ALTER TABLE `port_scans` DISABLE KEYS */;
INSERT INTO `port_scans` VALUES (1,'2024-01-23 16:15:46','192.168.100.1'),(2,'2024-01-24 16:22:35','192.168.100.1'),(3,'2024-01-24 16:27:07','192.168.100.1');
/*!40000 ALTER TABLE `port_scans` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping routines for database 'netauditsec'
--
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2024-01-24 16:32:39
