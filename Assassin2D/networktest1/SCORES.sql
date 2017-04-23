CREATE TABLE `scores` (
`id` INT( 10 ) NOT NULL AUTO_INCREMENT PRIMARY KEY ,
`name` VARCHAR( 30 ) NOT NULL ,
`password` VARCHAR( 50 ) NOT NULL,
`shop` INT(2),
`highscore1` INT( 4 ),
`highscore2` INT( 4 ),
`achievement` INT(1),
`experience` INT(3)
) ENGINE = innodb;

INSERT INTO `scores` ( `id` , `name` , `password`, `shop`, `highscore1`, `highscore2`, `achievement`, `experience`)
VALUES (
NULL , 'blake', MD5( 'blake' ), 0, 0, 0, 0, 0
);

CREATE TABLE `high` (
`id` INT( 10 ) NOT NULL AUTO_INCREMENT PRIMARY KEY ,
`name` VARCHAR( 30 ) NOT NULL ,
`name1` VARCHAR( 30 ) NOT NULL ,
`score1` INT( 4 ),
`name2` VARCHAR( 30 ) NOT NULL ,
`score2` INT( 4 ),
`name3` VARCHAR( 30 ) NOT NULL ,
`score3` INT( 4 ),
`name4` VARCHAR( 30 ) NOT NULL ,
`score4` INT( 4 ),
`name5` VARCHAR( 30 ) NOT NULL ,
`score5` INT( 4 ),
`name6` VARCHAR( 30 ) NOT NULL ,
`score6` INT( 4 ),
`name7` VARCHAR( 30 ) NOT NULL ,
`score7` INT( 4 ),
`name8` VARCHAR( 30 ) NOT NULL ,
`score8` INT( 4 ),
`name9` VARCHAR( 30 ) NOT NULL ,
`score9` INT( 4 ),
`name10` VARCHAR( 30 ) NOT NULL ,
`score10` INT( 4 )
) ENGINE = innodb;

INSERT INTO `high` ( `id`, `name`, `name1`, `score1`, `name2`, `score2`, `name3`, `score3`,
`name4` , `score4`, `name5` , `score5`, `name6` , `score6`, `name7` , `score7`,
`name8` , `score8`, `name9` , `score9`, `name10` , `score10` )
VALUES (
NULL , 'highscore1', '', 0, '', 0, '', 0, '', 0, '', 0, '', 0, '', 0, '', 0, '', 0, '', 0
);

INSERT INTO `high` ( `id`, `name`, `name1`, `score1`, `name2`, `score2`, `name3`, `score3`,
`name4` , `score4`, `name5` , `score5`, `name6` , `score6`, `name7` , `score7`,
`name8` , `score8`, `name9` , `score9`, `name10` , `score10` )
VALUES (
NULL , 'highscore2', '', 0, '', 0, '', 0, '', 0, '', 0, '', 0, '', 0, '', 0, '', 0, '', 0
);