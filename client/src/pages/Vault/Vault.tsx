import React, {useEffect, useState} from 'react';
import PageTitle from "../../components/PageTitile/PageTitle";
import {Button, Row, Spinner} from "reactstrap";
import AddItemModal from "./AddItemModal";
import VerifyMasterPassword from "./VerifyMasterPassword";
import CipherLogin from "../../components/CipherLogin/CipherLogin";
import * as api from "../../api/apiCalls";
import {useSelector} from "react-redux";
import {AuthState} from "../../redux/authenticationReducer";

export interface VaultItem {
    id: number
    url: string,
    encryptedPassword: string,
    identifier: string
}

const Vault = () => {
    const [isAddItemModalOpen, setAddItemModalOpen] = useState<boolean>(false);
    const toggleAddItemModal = () => setAddItemModalOpen(!isAddItemModalOpen);
    const [vault, setVault] = useState<VaultItem[]>([]);
    const [componentDidMount, setComponentDidMount] = useState<boolean>(false);
    const [error, setError] = useState<string>('');
    const [ongoingApiCall, setOngoingApiCall] = useState<boolean>(true);

    const encryptionKey = useSelector((state: AuthState) => state).encryptionKey;


    useEffect(() => {
        if (!componentDidMount) {
            setError('');
            setOngoingApiCall(true);
            api.getVault()
                .then(response => {
                    setComponentDidMount(true);
                    setOngoingApiCall(false);
                    setVault(response.data.items);
                })
                .catch(() => {
                    setOngoingApiCall(false)
                    setError("An error occurred, please try again later");
                });
        }
    }, [componentDidMount, vault]);

    return (
        <div>
            <PageTitle title="Your vault"/>
            <VerifyMasterPassword/>
            <AddItemModal isOpen={isAddItemModalOpen} toggle={toggleAddItemModal}
                          afterAdd={() => setComponentDidMount(false)}
            />
            {ongoingApiCall && !error && <div className="text-center mt-3"><Spinner animation="border" size="sm" role="status">
                Loading...
            </Spinner></div>}
            {error && <div className="text-center text-danger mt-4">{error}</div>}
            {!error && !ongoingApiCall &&  encryptionKey && <Row className="m-3">
                <div>
                    <div><Button className=" mt-4 py-1" style={{backgroundColor: "#d37e29"}} onClick={toggleAddItemModal}>Add password</Button></div>
                </div>
                <div className="mt-3">
                    {vault.length === 0 && !error && !ongoingApiCall &&
                    <div className="text-center">You have not saved any passwords</div>}
                    {vault.length > 0 && !error && !ongoingApiCall && <div>
                        {vault.map(item => (<CipherLogin
                            key={item.id}
                            id={item.id}
                            url={item.url}
                            encryptedPassword={item.encryptedPassword}
                            identifier={item.identifier}
                        />))}
                    </div>}
                </div>
            </Row>}
        </div>
    );
};

export default Vault;