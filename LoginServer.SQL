/*
SQLyog Community v12.5.0 (64 bit)
MySQL - 10.1.29-MariaDB : Database - masterserver
*********************************************************************
*/

/*!40101 SET NAMES utf8 */;

/*!40101 SET SQL_MODE=''*/;

/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;
CREATE DATABASE /*!32312 IF NOT EXISTS*/`masterserver` /*!40100 DEFAULT CHARACTER SET latin1 */;

USE `masterserver`;

/*Table structure for table `users` */

DROP TABLE IF EXISTS `users`;

CREATE TABLE `users` (
  `id` bigint(20) NOT NULL,
  `username` varchar(20) NOT NULL,
  `status` tinyint(10) NOT NULL DEFAULT '0',
  `displayname` varchar(16) NOT NULL,
  `password` char(64) NOT NULL,
  `passwordsalt` char(10) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

/*Data for the table `users` */

insert  into `users`(`id`,`username`,`status`,`displayname`,`password`,`passwordsalt`) values 
(0,'darkraptor',1,'DarkRaptor','7710EB97FF9D999D12B4AF7729920B7422EB0C58D9ACD552CBA27888EA7D0C76','aaaaaaaaaa'),
(1,'test',1,'Mono1','C036F6A9EBB0308EC638492371227F00F134EA6EE43AEA1337DF38474253ACAA','aaaaaaaaaa');

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;
