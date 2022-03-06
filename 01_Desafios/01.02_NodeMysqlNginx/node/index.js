const express = require('express')
const mysql = require('mysql')

const app = express()
const port = 3000

const config = {
    host: 'mysql',
    user: 'root',
    password: 'root',
    database: 'nodedb'
};

app.get('/', (req, res) => {

    const connection = mysql.createConnection(config);

    const qryCreateTable  = `create table if not exists people (id int auto_increment primary key, name varchar(255) not null);`;
    connection.beginTransaction();
    connection.query(qryCreateTable);
    connection.commit();

    const arrayOfNames = ["Kananda", "Yasmin", "Mael", "Ayla", "Amaterasu", "Merlin", "Scanor", "Baki", "Elfen Lied", "Kyojuro"];
    var index = Math.floor(Math.random() * 10);
    var personName = arrayOfNames[index];

    const qryInsertPerson  = `insert into people (name) values ('${personName}')`;
    connection.beginTransaction();
    connection.query(qryInsertPerson);
    connection.commit();

    const qryPeople = `select * from people;`;  
    connection.beginTransaction();
    connection.query(qryPeople, function (error, results, fields) {
        if (error)
            throw error;
         
        let resultList = "";
        results.forEach(element => {
            resultList += `<li>${element.name}</li>`;
        });

        let tableList = '<ul>' + resultList + '</ul>';
        console.log("get: " + tableList);
        if (!resultList)
            tableList = "Sem resultados para exibir";

        res.send('<h1>Full Cycle Rocks!</h1><br>' + tableList);
    });

    connection.commit();
    connection.end();
})

app.listen(port, () => {
    console.log('Rodando na porta ' + port)
})