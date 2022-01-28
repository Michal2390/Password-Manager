import React from 'react';
import {Input} from "reactstrap";


const PasswordInput = (props: {
    password: string, onChangePassword: (ev: React.ChangeEvent<HTMLInputElement>) => void, id: string, name: string, placeholder: string
}) => {

    return (
        <div>
            <Input type={"password"} onChange={props.onChangePassword} id={props.id} placeholder={props.placeholder}
                   name={props.name} value={props.password}/>
        </div>
    );
};

export default PasswordInput;