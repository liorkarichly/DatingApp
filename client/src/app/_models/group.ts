export interface Group{

    name:string;
    connections:Connection[];
}

interface Connection{
    conecctionId:string;
    username:string;
}