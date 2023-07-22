import React, { Component } from 'react';
import Button from '@mui/joy/Button';
import List from '@mui/joy/List';
import Typography from '@mui/joy/Typography';
import { ListItemButton, ListItem } from '../node_modules/@mui/joy/index';

class Game extends Component {
    constructor(props) {
        super(props);
        this.state = { selectedPlayer : "" }
    }

    componentDidMount() {
        this.setState({ ready: true });
    }

    renderPlayerList() {
        let entries = Object.entries(this.props.game.roles);
        entries = entries.sort(ele => Math.random() - 0.5);
    return<List>
        {entries.map(([key, val]) => <ListItemButton onClick={() => this.setSelectedPlayer(key)}> {key}</ListItemButton >)}
        <ListItemButton onClick={() => this.setSelectedPlayer("DoNotOpen")}>DoNotOpen</ListItemButton>
        </List>
    }

    renderRole(playerInfo) {
        console.log(playerInfo);
        return <div>
            <Typography>You are {playerInfo.roleName}. You are on the {playerInfo.alignment} team. </Typography> You have the following information: {playerInfo.information.map(ele => <ListItem> {ele}</ListItem>)} </div>
    }
    setSelectedPlayer(player) {
        this.setState({ selectedPlayer : player })
    } 

    renderDoNotOpen() {
        let entries = Object.entries(this.props.game.roles);
        return <div>
            {entries.map(([key, ele])=> {
                return <div> <Typography> {key} : {ele.roleName} </Typography>
                    <List> { ele.information.map(i => <ListItem> {i}</ListItem>) } </List> </div>
            }) }
        </div>
    }
    render() {
        let content = this.state.selectedPlayer === "" ? this.renderPlayerList() : <div> {this.state.selectedPlayer === "DoNotOpen" ? this.renderDoNotOpen() : this.renderRole(this.props.game.roles[this.state.selectedPlayer])} <Button sx={{ margin: "10px" }} onClick={() => this.setSelectedPlayer("")}>Back</Button> </div>
        return (
            <div>
                {this.state.ready ? <div>
                    <Typography level="h5" component="h2">Game {this.props.game.gameId}</Typography>
                    {content}
                    <Button sx={{margin: "10px"}}  onClick={() => this.props.exitGame()}>Exit Game</Button>
            </div>
                    : <div> Loading... </div>}
            </div>
        );
    }

}
export default Game;
