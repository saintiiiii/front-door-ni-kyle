import React, { createContext, useContext, useEffect, useState } from 'react';
import { createRoot } from 'react-dom/client';
import { BrowserRouter, Link, Navigate, Route, Routes, useLocation, useNavigate } from 'react-router-dom';
import { Wrench, ArrowRight, LogOut, UserRound, Menu, X } from 'lucide-react';
import { api } from './services/api';
import { Home, ServicesPage, InfoPage } from './pages/PublicPages';
import { LoginPage, RegisterPage } from './pages/AuthPages';
import { CustomerDashboard } from './pages/CustomerDashboard';
import { TechnicianDashboard, AdminDashboard } from './pages/Workspaces';
import { Loading } from './ui';
import { PasswordPanel } from './pages/PasswordPanel';
import './style.css';

export type Role='Customer'|'Technician'|'Administrator';
export type User={id:string;email:string;phoneNumber?:string;firstName:string;lastName:string;roles:Role[];technicianStatus?:string};
export const AuthContext=createContext<{user:User|null;setUser:(u:User|null)=>void;reload:()=>Promise<void>}>({user:null,setUser:()=>{},reload:async()=>{}});
export const useAuth=()=>useContext(AuthContext);

function Shell({children}:{children:React.ReactNode}){const {user,setUser}=useAuth();const [open,setOpen]=useState(false);const nav=useNavigate();async function signout(){try{await api('/api/auth/logout',{method:'POST'});}catch{}setUser(null);nav('/');}return <><header><Link className="brand" to="/"><span className="logo"><Wrench size={21}/></span><span>FIXIT<small>Fast In-home eXpert Installation and Technical Service</small></span></Link><button className="mobile-menu" aria-label="Toggle navigation" onClick={()=>setOpen(!open)}>{open?<X/>:<Menu/>}</button><nav className={open?'nav-open':''}><Link to="/services">Services</Link><a href="/#how">How it works</a><Link to="/about">About</Link>{user&&<Link to="/dashboard">Dashboard</Link>}</nav><div className="actions">{user?<><span className="user-pill"><UserRound size={15}/>{user.firstName}</span><button className="login" onClick={signout} title="Sign out"><LogOut size={16}/></button></>:<><Link className="login" to="/login">Log in</Link><Link className="button small" to="/register">Get started <ArrowRight size={16}/></Link></>}</div></header>{children}<footer><Link className="brand" to="/"><span className="logo"><Wrench size={18}/></span><span>FIXIT<small>Home repairs, made easier.</small></span></Link><span>Built for better homes across the Philippines.</span><span><Link to="/privacy">Privacy</Link> · <Link to="/terms">Terms</Link> · <Link to="/safety">Safety</Link> · <Link to="/faq">FAQ</Link></span><span>Demo platform · Payments are simulated</span></footer></>}
function RequireAuth({children}:{children:React.ReactNode}){const {user}=useAuth();const loc=useLocation();return user?<>{children}</>:<Navigate to="/login" state={{from:loc.pathname}} replace/>;}
function Dashboard(){const {user}=useAuth();if(!user)return <Navigate to="/login"/>;return user.roles.includes('Administrator')?<><AdminDashboard/><main className="dashboard"><PasswordPanel/></main></>:user.roles.includes('Technician')?<><TechnicianDashboard/><main className="dashboard"><PasswordPanel/></main></>:<CustomerDashboard/>;}
function App(){const [user,setUser]=useState<User|null>(null);const [ready,setReady]=useState(false);const reload=async()=>{try{setUser(await api<User>('/api/auth/me'));}catch{setUser(null);}finally{setReady(true);}};useEffect(()=>{reload();},[]);return <AuthContext.Provider value={{user,setUser,reload}}><BrowserRouter><Shell><Routes><Route path="/" element={<Home/>}/><Route path="/services" element={<ServicesPage/>}/><Route path="/login" element={user?<Navigate to="/dashboard"/>:<LoginPage/>}/><Route path="/register" element={user?<Navigate to="/dashboard"/>:<RegisterPage/>}/><Route path="/dashboard" element={ready?<RequireAuth><Dashboard/></RequireAuth>:<main className="page-wrap"><Loading/></main>}/><Route path="/about" element={<InfoPage type="about"/>}/><Route path="/privacy" element={<InfoPage type="privacy"/>}/><Route path="/terms" element={<InfoPage type="terms"/>}/><Route path="/safety" element={<InfoPage type="safety"/>}/><Route path="/faq" element={<InfoPage type="faq"/>}/><Route path="/cancellation" element={<InfoPage type="cancellation"/>}/><Route path="*" element={<InfoPage type="about"/>}/></Routes></Shell></BrowserRouter></AuthContext.Provider>}

createRoot(document.getElementById('root')!).render(<React.StrictMode><App/></React.StrictMode>);
