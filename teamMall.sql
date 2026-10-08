-- MySQL dump 10.13  Distrib 8.0.19, for Win64 (x86_64)
--
-- Host: localhost    Database: teammall
-- ------------------------------------------------------
-- Server version	8.0.46

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
-- Table structure for table `inasistencia`
--

DROP TABLE IF EXISTS `inasistencia`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `inasistencia` (
  `IdInasistencia` int NOT NULL AUTO_INCREMENT,
  `IdUsuario` int NOT NULL,
  `Tipo` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `FechaAusencia` date NOT NULL,
  `FechaCarga` datetime NOT NULL,
  `Archivo` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Comentario` text COLLATE utf8mb4_unicode_ci,
  `Estado` varchar(30) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'Pendiente',
  PRIMARY KEY (`IdInasistencia`),
  KEY `IX_Inasistencia_IdUsuario` (`IdUsuario`),
  KEY `IX_Inasistencia_FechaAusencia` (`FechaAusencia`),
  KEY `IX_Inasistencia_Tipo` (`Tipo`),
  KEY `IX_Inasistencia_Estado` (`Estado`),
  CONSTRAINT `FK_Inasistencia_Usuario` FOREIGN KEY (`IdUsuario`) REFERENCES `usuario` (`IdUsuario`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `inasistencia`
--

LOCK TABLES `inasistencia` WRITE;
/*!40000 ALTER TABLE `inasistencia` DISABLE KEYS */;
INSERT INTO `inasistencia` VALUES (1,2,'Certificado','2026-10-01','2026-10-04 11:41:38',NULL,'Certificado médico presentado.','Pendiente'),(2,3,'Justificacion','2026-10-02','2026-10-04 11:41:38',NULL,'Justificación de inasistencia.','Revisado');
/*!40000 ALTER TABLE `inasistencia` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `local`
--

DROP TABLE IF EXISTS `local`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `local` (
  `IdLocal` int NOT NULL AUTO_INCREMENT,
  `Nombre` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Activo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`IdLocal`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `local`
--

LOCK TABLES `local` WRITE;
/*!40000 ALTER TABLE `local` DISABLE KEYS */;
INSERT INTO `local` VALUES (1,'Adidas',1),(2,'Nike',1),(3,'Todo Moda',1),(4,'Macowens',1),(5,'Kevingston',1);
/*!40000 ALTER TABLE `local` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `notificacion`
--

DROP TABLE IF EXISTS `notificacion`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `notificacion` (
  `IdNotificacion` int NOT NULL AUTO_INCREMENT,
  `IdUsuario` int NOT NULL,
  `IdPublicacion` int NOT NULL,
  `FechaCreacion` datetime NOT NULL,
  `FechaLectura` datetime DEFAULT NULL,
  `Leida` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`IdNotificacion`),
  KEY `IX_Notificacion_IdUsuario` (`IdUsuario`),
  KEY `IX_Notificacion_IdPublicacion` (`IdPublicacion`),
  KEY `IX_Notificacion_Leida` (`Leida`),
  CONSTRAINT `FK_Notificacion_Publicacion` FOREIGN KEY (`IdPublicacion`) REFERENCES `publicacion` (`IdPublicacion`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `FK_Notificacion_Usuario` FOREIGN KEY (`IdUsuario`) REFERENCES `usuario` (`IdUsuario`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `notificacion`
--

LOCK TABLES `notificacion` WRITE;
/*!40000 ALTER TABLE `notificacion` DISABLE KEYS */;
INSERT INTO `notificacion` VALUES (1,2,1,'2026-10-04 11:41:38',NULL,0),(2,3,1,'2026-10-04 11:41:38',NULL,0),(3,2,2,'2026-10-04 11:41:38',NULL,0),(4,4,2,'2026-10-04 11:41:38',NULL,0),(5,2,3,'2026-10-04 11:41:38',NULL,0);
/*!40000 ALTER TABLE `notificacion` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `publicacion`
--

DROP TABLE IF EXISTS `publicacion`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `publicacion` (
  `IdPublicacion` int NOT NULL AUTO_INCREMENT,
  `Titulo` varchar(200) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Descripcion` text COLLATE utf8mb4_unicode_ci NOT NULL,
  `Tipo` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Archivo` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `FechaPublicacion` datetime NOT NULL,
  `IdUsuario` int NOT NULL,
  PRIMARY KEY (`IdPublicacion`),
  KEY `IX_Publicacion_IdUsuario` (`IdUsuario`),
  CONSTRAINT `FK_Publicacion_Usuario` FOREIGN KEY (`IdUsuario`) REFERENCES `usuario` (`IdUsuario`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `publicacion`
--

LOCK TABLES `publicacion` WRITE;
/*!40000 ALTER TABLE `publicacion` DISABLE KEYS */;
INSERT INTO `publicacion` VALUES (1,'Nuevo manual de atención al cliente','Se encuentra disponible el nuevo manual de atención al cliente.','Manual',NULL,'2026-10-04 11:41:38',1),(2,'Capacitación de ventas','Material correspondiente a la nueva capacitación de ventas.','Capacitacion',NULL,'2026-10-04 11:41:38',1),(3,'Nueva comunicación interna','Recordatorio sobre los procedimientos internos del shopping.','Novedad',NULL,'2026-10-04 11:41:38',1);
/*!40000 ALTER TABLE `publicacion` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `publicacionlocal`
--

DROP TABLE IF EXISTS `publicacionlocal`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `publicacionlocal` (
  `IdPublicacion` int NOT NULL,
  `IdLocal` int NOT NULL,
  PRIMARY KEY (`IdPublicacion`,`IdLocal`),
  KEY `FK_PublicacionLocal_Local` (`IdLocal`),
  CONSTRAINT `FK_PublicacionLocal_Local` FOREIGN KEY (`IdLocal`) REFERENCES `local` (`IdLocal`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `FK_PublicacionLocal_Publicacion` FOREIGN KEY (`IdPublicacion`) REFERENCES `publicacion` (`IdPublicacion`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `publicacionlocal`
--

LOCK TABLES `publicacionlocal` WRITE;
/*!40000 ALTER TABLE `publicacionlocal` DISABLE KEYS */;
INSERT INTO `publicacionlocal` VALUES (1,1),(2,1),(3,1),(1,2),(3,2),(2,3),(3,3);
/*!40000 ALTER TABLE `publicacionlocal` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `usuario`
--

DROP TABLE IF EXISTS `usuario`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `usuario` (
  `IdUsuario` int NOT NULL AUTO_INCREMENT,
  `DNI` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Nombre` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Apellido` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Email` varchar(150) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Contrasenia` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Rol` varchar(30) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Avatar` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `IdLocal` int DEFAULT NULL,
  `Activo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`IdUsuario`),
  UNIQUE KEY `UQ_Usuario_DNI` (`DNI`),
  UNIQUE KEY `UQ_Usuario_Email` (`Email`),
  KEY `IX_Usuario_IdLocal` (`IdLocal`),
  CONSTRAINT `FK_Usuario_Local` FOREIGN KEY (`IdLocal`) REFERENCES `local` (`IdLocal`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `usuario`
--

LOCK TABLES `usuario` WRITE;
/*!40000 ALTER TABLE `usuario` DISABLE KEYS */;
INSERT INTO `usuario` VALUES (1,'30000001','Ana','Administrador','admin@teammall.com','CAMBIAR_HASH_ADMIN','Administrador',NULL,NULL,1),(2,'30000002','Carla','Gomez','carla@teammall.com','CAMBIAR_HASH_VENDEDORA','Vendedora',NULL,1,1),(3,'30000003','Lucia','Fernandez','lucia@teammall.com','CAMBIAR_HASH_VENDEDORA','Vendedora',NULL,2,1),(4,'30000004','Sofia','Martinez','sofia@teammall.com','CAMBIAR_HASH_VENDEDORA','Vendedora',NULL,3,1);
/*!40000 ALTER TABLE `usuario` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping routines for database 'teammall'
--
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-10-08 16:16:52
