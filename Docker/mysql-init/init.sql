-- MySQL dump 10.13  Distrib 8.0.41, for Linux (x86_64)
--
-- Host: localhost    Database: ExnatonDB
-- ------------------------------------------------------
-- Server version	8.0.41

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `measurements`
--

DROP TABLE IF EXISTS `measurements`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `measurements` (
  `Id` char(36) NOT NULL,
  `measurement` varchar(255) NOT NULL,
  `rectimestamp` datetime NOT NULL,
  `tagsMUId` char(36) NOT NULL,
  `Indications` json DEFAULT NULL,
  PRIMARY KEY (`Id`,`rectimestamp`),
  UNIQUE KEY `idx_measurement_timestamp_muid` (`measurement`,`rectimestamp`,`tagsMUId`),
  KEY `idx_measurement_tags_time` (`measurement`,`tagsMUId`,`rectimestamp`),
  KEY `idx_time` (`rectimestamp`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci
/*!50100 PARTITION BY RANGE (((year(`rectimestamp`) * 100) + month(`rectimestamp`)))
(PARTITION p202201 VALUES LESS THAN (202202) ENGINE = InnoDB,
 PARTITION p202202 VALUES LESS THAN (202203) ENGINE = InnoDB,
 PARTITION p202203 VALUES LESS THAN (202204) ENGINE = InnoDB,
 PARTITION p202204 VALUES LESS THAN (202205) ENGINE = InnoDB,
 PARTITION p202205 VALUES LESS THAN (202206) ENGINE = InnoDB,
 PARTITION p202206 VALUES LESS THAN (202207) ENGINE = InnoDB,
 PARTITION p202207 VALUES LESS THAN (202208) ENGINE = InnoDB,
 PARTITION p202208 VALUES LESS THAN (202209) ENGINE = InnoDB,
 PARTITION p202209 VALUES LESS THAN (202210) ENGINE = InnoDB,
 PARTITION p202210 VALUES LESS THAN (202211) ENGINE = InnoDB,
 PARTITION p202211 VALUES LESS THAN (202212) ENGINE = InnoDB,
 PARTITION p202212 VALUES LESS THAN (202301) ENGINE = InnoDB,
 PARTITION p202301 VALUES LESS THAN (202302) ENGINE = InnoDB,
 PARTITION p202302 VALUES LESS THAN (202303) ENGINE = InnoDB,
 PARTITION p_max VALUES LESS THAN MAXVALUE ENGINE = InnoDB) */;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `measurements`
--

LOCK TABLES `measurements` WRITE;
/*!40000 ALTER TABLE `measurements` DISABLE KEYS */;
/*!40000 ALTER TABLE `measurements` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `tags`
--

DROP TABLE IF EXISTS `tags`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tags` (
  `Id` char(36) NOT NULL,
  `muid` char(36) NOT NULL,
  `quality` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `muid` (`muid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tags`
--

LOCK TABLES `tags` WRITE;
/*!40000 ALTER TABLE `tags` DISABLE KEYS */;
INSERT INTO `tags` VALUES ('6e1f0c67-b5ff-4b43-9fa1-8c04c42e84c6','1db7649e-9342-4e04-97c7-f0ebb88ed1f8','measured'),('ce6fabd7-3e19-4a2b-b02a-3a4ab5898019','95ce3367-cbce-4a4d-bbe3-da082831d7bd','measured');
/*!40000 ALTER TABLE `tags` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-04-01 17:16:21
