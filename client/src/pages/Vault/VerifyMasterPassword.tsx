import React, {Dispatch, useState} from 'react';
import {FormGroup, Label, Modal, ModalBody, ModalFooter, ModalHeader} from "reactstrap";
import ButtonWithSpinner from "../../components/ButtonWithSpinner/ButtonWithSpinner";
import * as api from "../../api/apiCalls";
import {derivativeKey, hashDerivationKey} from "../../security/KeyDerivation";
import {useDispatch, useSelector} from "react-redux";
import {Action, AuthState} from "../../redux/authenticationReducer";
import PasswordInput from "../../components/PasswordInput/PasswordInput";

const VerifyMasterPassword = () => {
        const [masterPassword, setMasterPassword] = useState<string>('');
        const [ongoingApiCall, setOngoingApiCall] = useState<boolean>(false);
        const [error, setError] = useState<string>('');
        const reduxState = useSelector((state: AuthState) => state);
        const dispatch: Dispatch<Action> = useDispatch();

        const onChangeMasterPassword = (ev: React.ChangeEvent<HTMLInputElement>) => {
            if (masterPassword !== ev.target.value.trim()) {
                setMasterPassword(ev.target.value.trim());
                setError('');
            }
        }

        const onClickVerify = () => {
            setOngoingApiCall(true);
            const encryptionKey = derivativeKey(masterPassword, reduxState.email);
            const encryptionKeyHash = hashDerivationKey(encryptionKey, reduxState.email);
            api.verifyEncryptionKey({encryptionKeyHash})
                .then(() => {
                    setOngoingApiCall(false);
                    dispatch({
                        type: "verify-master-password",
                        payload: {...reduxState, encryptionKey}
                    })
                })
                .catch(error => {
                        setOngoingApiCall(false);
                        if (error?.response?.data) {
                            setError("Incorrect master password");
                            return;
                        }
                        setError("An error occurred, please try again later")
                    }
                )
        }

        return (
            <Modal isOpen={!reduxState.encryptionKey} toggle={() => {
                return;
            }} centered>
                <ModalHeader toggle={() => {
                    return;
                }}>
                    Verify your master password </ModalHeader>
                <ModalBody>
                    <FormGroup>
                        <Label for="password" className="mt-3">
                            Master password:
                        </Label>
                        <PasswordInput
                            id="password"
                            name="Passw vord"
                            placeholder="Enter your master password"
                            password={masterPassword}
                            onChangePassword={onChangeMasterPassword}
                        />
                    </FormGroup>

                </ModalBody>
                {error && <div className="text-danger text-center mt-1 mb-3">{error}</div>}
                <ModalFooter>
                    <ButtonWithSpinner onClick={onClickVerify} disabled={masterPassword.length < 8} className=""
                                       content={"Verify"} ongoingApiCall={ongoingApiCall}/>
                </ModalFooter>
            </Modal>
        );
    }
;

export default VerifyMasterPassword;