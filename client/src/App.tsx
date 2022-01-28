import {useState} from "react";
import SideBar from "./components/Sidebar/Sidebar";
import {BrowserRouter, Navigate, Route, Routes} from "react-router-dom";
import {Col, Row} from "reactstrap";
import Vault from "./pages/Vault/Vault";
import Settings from "./pages/Settings/Settings";
import Register from "./pages/Register/Register";
import Login from "./pages/Login/Login";
import RequestResetPassword from "./pages/RequestResetPassword/RequestResetPassword";
import ResetPassword from "./pages/ResetPassword/ResetPassword";
import VerifyEmailAddress from "./pages/VerifyEmail/VerifyEmailAddress";
import {useSelector} from "react-redux";
import {AuthState} from "./redux/authenticationReducer";
import RequestEmailVerification from "./pages/RequestEmailVerification/RequestEmailVerification";

function App() {
    const [sidebarIsOpen, setSidebarOpen] = useState(true);
    const toggleSidebar = () => setSidebarOpen(!sidebarIsOpen);

    const isUserLoggedIn = useSelector((state: AuthState) => state.isLoggedIn);

    return (
        <BrowserRouter>
            <div className="m-0 p-0">
                {!isUserLoggedIn && <Routes>
                    <Route path="/login" element={<Login/>}/>
                    <Route path="/register" element={<Register/>}/>
                    <Route path="/request-reset-password" element={<RequestResetPassword/>}/>
                    <Route path="/request-email-verification" element={<RequestEmailVerification/>}/>
                    <Route path="/reset-password/:username/:token" element={<ResetPassword/>}/>
                    <Route path="/verify-email-address/:username/:token" element={<VerifyEmailAddress/>}/>
                    <Route path="/*" element={<Navigate to="/login"/>}/>
                </Routes>}
                {isUserLoggedIn && <Row className="m-0">
                    <Col lg="10" md="9" sm="8" className="p-0">
                        <Routes>
                            <Route path="/passwords" element={<Vault/>}/>
                            <Route path="/change-password" element={<Settings/>}/>
                            <Route path="/*" element={<Navigate to="/passwords"/>}/>
                        </Routes>
                    </Col>
                    <SideBar toggle={toggleSidebar} isOpen={sidebarIsOpen}/>
                </Row>}
            </div>
        </BrowserRouter>
    );
}

export default App;
