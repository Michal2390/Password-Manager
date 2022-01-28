import React, {Dispatch} from "react";
import {Col, Nav, NavItem, NavLink} from "reactstrap";
import classNames from "classnames";
import {Link} from "react-router-dom";
import * as api from "../../api/apiCalls";
import {useDispatch} from "react-redux";
import {Action} from "../../redux/authenticationReducer";

const SideBar = (props: any) => {
    const dispatch: Dispatch<Action> = useDispatch();

    const onClickLogoutButton = () => {
        dispatch({type: "logout"});
        api.deleteAuthHeader();
    }

    return (
        <Col lg="2" md="3" sm="4"
             className={classNames("sidebar", {"is-open": props.isOpen}, "side-menu", "p-0", "m-0")}>
            <div className="sidebar-header mt-4 text-center mb-3">
                <h3>Secure Password Manager</h3>
            </div>
            <div>
                <Nav vertical className="list-group pb-3">
                    <NavItem>
                        <NavLink tag={Link} to={"/passwords"} className="menu-item">
                            See passwords
                        </NavLink>
                    </NavItem>

                    <NavItem>
                        <NavLink tag={Link} to={"/change-password"} className="menu-item">
                            Change password
                        </NavLink>
                    </NavItem>
                    <div className="text-center logout-button ms-1 px-2 py-1 mt-5 menu-item" onClick={onClickLogoutButton}>
                        <small>
                            Log out
                        </small>
                    </div>
                </Nav>
            </div>
        </Col>
    );
}

export default SideBar;
