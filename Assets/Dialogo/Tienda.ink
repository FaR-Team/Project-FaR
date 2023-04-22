-> main
 
=== main ===
Esto debería de ser una tienda, pero sasha no me dejó
...
Ya que estás acá, ¿Te puedo preguntar algo?
    + [Sí]
        -> chosen("Sí")
    + [No]
        -> chosen("No")
=== chosen(answer) ===
¿Sabés hablar en Swahili?

    + [I DON'T KNOW SWAHILI]
        -> chosen2("I DON'T KNOW SWAHILI")
    + [I DON'T KNOW SWAHILI]
        -> chosen2("I DON'T KNOW SWAHILI")
    + [I DON'T KNOW SWAHILI]
        -> chosen2("I DON'T KNOW SWAHILI")
    + [I DON'T KNOW SWAHILI]
        -> chosen2("I DON'T KNOW SWAHILI")
    + [I DON'T KNOW SWAHILI]
        -> chosen2("I DON'T KNOW SWAHILI")
    + [I DON'T KNOW SWAHILI]
        -> chosen2("I DON'T KNOW SWAHILI")
        
=== chosen2(answer) ===
Fua loco, nadie sabe una poronga en esta alpha de mierda
-> END