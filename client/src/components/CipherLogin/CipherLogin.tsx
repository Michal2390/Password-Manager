import React, {useState} from 'react';
import {VaultItem} from "../../pages/Vault/Vault";
import {decryptPassword} from "../../security/Encryption";
import {useSelector} from "react-redux";
import {AuthState} from "../../redux/authenticationReducer";

const CipherLogin = (props: VaultItem) => {
    const [, setIsModalOpen] = useState<boolean>(false);
    const [isPasswordRevealed, setIsPasswordRevealed] = useState<boolean>(false);
    const encryptionKey = useSelector((state: AuthState) => state.encryptionKey)

    function onClickItem() {
        setIsPasswordRevealed(false);
        setIsModalOpen(true);
    }

    function revealPassword() {
        if (isPasswordRevealed) {
            return decryptPassword(props.encryptedPassword, encryptionKey);
        }
    }

    return (
        <div onClick={() => setIsPasswordRevealed(!isPasswordRevealed)}>
            <div className="p-2" style={{cursor: "pointer"}} onClick={onClickItem}>
                <div className="mt-4">
                    <h6 className="fw-bold">{props.url}</h6>
                    <div><small className="text-muted">username or email: <span
                        className="fw-bold">{props.identifier}</span></small></div>
                    {!isPasswordRevealed && <div><small className="text-muted">password: <span
                        className="fw-bold">*****************</span></small>
                    </div>}
                    {isPasswordRevealed && <div><small className="text-muted">password: <span
                        className="fw-bold">{revealPassword()}</span></small>
                    </div>}
                </div>
            </div>
        </div>
    );
};

export default CipherLogin;